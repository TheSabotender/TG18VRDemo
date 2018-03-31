/****************************************************/
//                Expose In Editor                  //
//                   Version 0.45                   //
//                by Thomas Beswick                 //
/****************************************************/
//              Now supports:                       //
//                  Int, Int[], Float,              //
//                  Float[], Bool, Bool[]           //
//                  String, String[],               //
//                  Vector2, Vector3                //
//                  GameObject, Texture2D           //
//                  Sprite                          //
/****************************************************/

using System;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEditor.SceneManagement;

[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class ExposeInEditorAttributeDrawer : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var type = target.GetType();

        foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        {
            var attributes = method.GetCustomAttributes(typeof(ExposeInEditorAttribute), true);
            if (attributes.Length > 0)
            {
                string nameOverride = (attributes[0] as ExposeInEditorAttribute).buttonNameOverride;
                bool showWarn = (attributes[0] as ExposeInEditorAttribute).showWarning;
                bool allowEdit = (attributes[0] as ExposeInEditorAttribute).allowInEditor;
                bool allowPlay = (attributes[0] as ExposeInEditorAttribute).allowWhilePlaying;

                Color c = Color.white;
                if (ColorUtility.TryParseHtmlString((attributes[0] as ExposeInEditorAttribute).buttonColorOverride.ToString(), out c))
                {
                    GUI.color = c;
                }

                if ((Application.isPlaying && allowPlay) || (!Application.isPlaying && allowEdit))
                {
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                    //EditorUtility.DisplayDialog("Warning", "Application is " + (Application.isPlaying ? "" : "not ") + "playing, and this method is not allowed.", "Ok");
                }

                if (GUILayout.Button(string.IsNullOrEmpty(nameOverride) ? method.Name : nameOverride))
                {
                    OnClick(showWarn, method);
                }

                GUI.enabled = true;
                GUI.color = Color.white;
            }
        }
    }

    void OnClick(bool showWarn, MethodInfo method)
    {
        if (!Application.isPlaying && showWarn)
        {
            if (EditorUtility.DisplayDialog("WARNING", "Application is not playing, are you sure you want to run: " + method.Name + "?", "Yes", "Cancel"))
            {
                DoInvoke(method);
            }
        }
        else
        {
            DoInvoke(method);
        }
    }

    void DoInvoke(MethodInfo method)
    {
        ParameterInfo[] parameters = method.GetParameters();
        if (parameters != null && parameters.Length > 0)
        {

            ExposeInEditorInputPrompt.Open(target, method, parameters);
            //ScriptableObject.CreateInstance<ExposeInEditorInputPrompt>();
        }
        else
        {
            ((MonoBehaviour)target).Invoke(method.Name, 0f);
            EditorUtility.SetDirty(target);
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(((MonoBehaviour)target).gameObject.scene);
            }
        }
    }
}

class ExposeInEditorInputPrompt : EditorWindow
{
    private Object target;
    private MethodInfo method;
    private ParameterInfo[] parameters;

    private object[] setParameters;

    public static void Open(Object target, MethodInfo method, ParameterInfo[] parameters)
    {
        var invoke = target.GetType();
        var invokeMethod = invoke.GetMethod(method.Name);
        if (invokeMethod == null)
        {
            Debug.LogError("ERROR:  " + invoke.FullName + "." + method.Name + " must be public for variables to work.");
            return;
        }

        ExposeInEditorInputPrompt ip = ExposeInEditorInputPrompt.GetWindow(typeof(ExposeInEditorInputPrompt), true) as ExposeInEditorInputPrompt;

        ip.target = target;
        ip.method = method;
        ip.parameters = parameters;
        ip.setParameters = new object[parameters.Length];

        //ip.ShowAsDropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), new Vector2(200,150));    
        ip.Show();
        ip.position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);
    }

    void OnGUI()
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            switch (parameters[i].ParameterType.ToString())
            {
                case "System.String":
                    setParameters[i] = EditorGUILayout.TextField(parameters[i].Name, setParameters[i] as string) as System.String;
                    break;
                case "System.String[]":
                    int saVal = 0;
                    if (setParameters[i] != null)
                    {
                        saVal = ((string[])setParameters[i]).Length;
                    }
                    else
                    {
                        setParameters[i] = new string[0];
                    }
                    saVal = EditorGUILayout.IntField(parameters[i].Name, saVal);
                    if (((string[])setParameters[i]).Length != saVal)
                    {
                        setParameters[i] = new string[saVal];
                    }
                    for (int sa = 0; sa < saVal; sa++)
                    {
                        ((string[])setParameters[i])[sa] = EditorGUILayout.TextField("- ", ((string[])setParameters[i])[sa]);
                    }
                    break;
                case "System.Int32":
                    int iVal = 0;
                    if (setParameters[i] != null)
                    {
                        iVal = (int)setParameters[i];
                    }
                    setParameters[i] = EditorGUILayout.IntField(parameters[i].Name, iVal);
                    break;
                case "System.Int32[]":
                    int iaVal = 0;
                    if (setParameters[i] != null)
                    {
                        iaVal = ((int[])setParameters[i]).Length;
                    }
                    else
                    {
                        setParameters[i] = new int[0];
                    }
                    iaVal = EditorGUILayout.IntField(parameters[i].Name, iaVal);
                    if (((int[])setParameters[i]).Length != iaVal)
                    {
                        setParameters[i] = new int[iaVal];
                    }
                    for (int ia = 0; ia < iaVal; ia++)
                    {
                        ((int[])setParameters[i])[ia] = EditorGUILayout.IntField("- ", ((int[])setParameters[i])[ia]);
                    }
                    break;
                case "System.Single":
                    float fVal = 0;
                    if (setParameters[i] != null)
                    {
                        fVal = (float)setParameters[i];
                    }
                    setParameters[i] = EditorGUILayout.FloatField(parameters[i].Name, fVal);

                    GUILayout.Label("Doesnt support type:\n" + parameters[i].ParameterType.ToString());
                    Debug.LogWarning("Doesnt support type: " + parameters[i].ParameterType.ToString());

                    break;
                case "System.Single[]":
                    int faVal = 0;
                    if (setParameters[i] != null)
                    {
                        faVal = ((float[])setParameters[i]).Length;
                    }
                    else
                    {
                        setParameters[i] = new float[0];
                    }
                    faVal = EditorGUILayout.IntField(parameters[i].Name, faVal);
                    if (((float[])setParameters[i]).Length != faVal)
                    {
                        setParameters[i] = new float[faVal];
                    }
                    for (int ia = 0; ia < faVal; ia++)
                    {
                        ((float[])setParameters[i])[ia] = EditorGUILayout.FloatField("- ", ((float[])setParameters[i])[ia]);
                    }
                    break;
                case "System.Boolean":
                    bool bVal = false;
                    if (setParameters[i] != null)
                    {
                        bVal = (bool)setParameters[i];
                    }
                    setParameters[i] = EditorGUILayout.Toggle(parameters[i].Name, bVal);
                    break;
                case "System.Boolean[]":
                    int baVal = 0;
                    if (setParameters[i] != null)
                    {
                        baVal = ((bool[])setParameters[i]).Length;
                    }
                    else
                    {
                        setParameters[i] = new bool[0];
                    }
                    baVal = EditorGUILayout.IntField(parameters[i].Name, baVal);
                    if (((bool[])setParameters[i]).Length != baVal)
                    {
                        setParameters[i] = new bool[baVal];
                    }
                    for (int ia = 0; ia < baVal; ia++)
                    {
                        ((bool[])setParameters[i])[ia] = EditorGUILayout.Toggle("- ", ((bool[])setParameters[i])[ia]);
                    }
                    break;
                case "UnityEngine.Vector2":
                    Vector2 v2Val = Vector2.zero;
                    if (setParameters[i] != null)
                    {
                        v2Val = (Vector2)setParameters[i];
                    }
                    setParameters[i] = EditorGUILayout.Vector2Field(parameters[i].Name, v2Val);
                    break;
                case "UnityEngine.Vector3":
                    Vector3 v3Val = Vector3.zero;
                    if (setParameters[i] != null)
                    {
                        v3Val = (Vector3)setParameters[i];
                    }
                    setParameters[i] = EditorGUILayout.Vector3Field(parameters[i].Name, v3Val);
                    break;
                case "UnityEngine.GameObject":
                    GameObject goVal = null;
                    if (setParameters[i] != null)
                    {
                        goVal = (GameObject)setParameters[i];
                    }
                    setParameters[i] = EditorGUILayout.ObjectField(parameters[i].Name, goVal, typeof(GameObject), true);
                    break;
                case "UnityEngine.Texture2D":
                    Texture2D t2dVal = null;
                    if (setParameters[i] != null)
                    {
                        t2dVal = (Texture2D)setParameters[i];
                    }
                    setParameters[i] = EditorGUILayout.ObjectField(parameters[i].Name, t2dVal, typeof(Texture2D), true);
                    break;
                case "UnityEngine.Sprite":
                    Sprite sVal = null;
                    if (setParameters[i] != null)
                    {
                        sVal = (Sprite)setParameters[i];
                    }
                    setParameters[i] = EditorGUILayout.ObjectField(parameters[i].Name, sVal, typeof(Sprite), true);
                    break;
                default:
                    GUILayout.Label("Doesnt support type:\n" + parameters[i].ParameterType.ToString());
                    Debug.LogWarning("Doesnt support type: " + parameters[i].ParameterType.ToString());
                    break;
            }
        }

        if (GUILayout.Button("Execute"))
        {
            var invoke = target.GetType();
            var invokeMethod = invoke.GetMethod(method.Name);

            if (invokeMethod == null)
            {
                Debug.LogError("Method " + method.Name + " could not be found on " + invoke.FullName);
                return;
            }

            invokeMethod.Invoke(target, setParameters);

            //((MonoBehaviour)target).Invoke(method.Name,parameters);
            EditorUtility.SetDirty(target);
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(((MonoBehaviour)target).gameObject.scene);
            }
            Close();
        }
    }
}