using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditor : EditorWindow
{

    private LevelSelect.Level[] levels;
    private string[] filePaths;
    private int selected = -1;
    private AssetRegistry registry;

	[MenuItem("Dark Codex/Level Editor")]
	public static void Init ()
	{
	    LevelEditor le = LevelEditor.GetWindow<LevelEditor>();
        le.Show();
	}

    void Check()
    {
        if (levels == null || levels.Length == 0 || filePaths == null || filePaths.Length != levels.Length)
        {
            string[] files = AssetDatabase.FindAssets("t:textasset", new string[1] { "Assets/Levels" });            
            Debug.Log("Found " + files.Length + " level files");

            levels = new LevelSelect.Level[files.Length];
            filePaths = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(files[i]));
                levels[i] = JsonUtility.FromJson<LevelSelect.Level>(asset.text);
                filePaths[i] = Application.dataPath + AssetDatabase.GUIDToAssetPath(files[i]).Remove(0, "Assets".Length);

                if (string.IsNullOrEmpty(levels[i].levelName))
                {
                    levels[i].levelName = asset.name;
                }

                if (levels[i].color.a == 0)
                {
                    levels[i].color.a = 1;
                }
            }

            selected = -1;
        }
        if (registry == null)
        {
            var g = Resources.Load<GameObject>("AssetRegistry");
            if (g != null)
            {
                if (g.GetComponent<AssetRegistry>())
                {
                    registry = g.GetComponent<AssetRegistry>();
                    Debug.Log("Found Registry");
                }                
            }
        }
    }

    void OnGUI()
    {
        Check();
        DrawMap();
        
        GUILayout.BeginHorizontal(GUILayout.Width(this.position.width), GUILayout.Height(this.position.height));

        //SELECT AREA
        GUILayout.BeginVertical((GUIStyle)"Box", GUILayout.Width(150));
        if (levels != null && levels.Length > 0)
        {
            for (var i = 0; i < levels.Length; i++)
            {
                if (GUILayout.Button(levels[i].levelName))
                {
                    selected = i;
                }
            }
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reload"))
        {
            levels = null;
            Check();
        }
        if (GUILayout.Button("Save All"))
        {
            for (var i = 0; i < levels.Length; i++)
            {
                CreateFile(levels[i], filePaths[i]);
            }
        }
        if (GUILayout.Button("Create Template"))
        {
            CreateFileFromLevel(new LevelSelect.Level(), "template");
        }
        GUILayout.EndVertical();

        //MAP AREA
        GUILayout.BeginVertical(GUILayout.Height(this.position.height), GUILayout.Width(this.position.width-150));

        GUILayout.Space(this.position.height - 200);
        GUILayout.BeginVertical((GUIStyle)"Box", GUILayout.Height(200));
        if (levels != null && levels.Length > 0 && selected != -1)
        {
            DrawInfo();
        }
        GUILayout.EndVertical();

        GUILayout.EndVertical();
        

        GUILayout.EndHorizontal();
    }

    private void DrawInfo()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        
        GUIStyle style = new GUIStyle((GUIStyle)"TextField");
        style.font = registry.fonts[levels[selected].font];
        style.fontSize = 32;
        levels[selected].levelName = EditorGUILayout.TextField("Name", levels[selected].levelName, style, GUILayout.Height(50)); //NAME
        style.fontSize = 16;
        levels[selected].subname = EditorGUILayout.TextField("Sub", levels[selected].subname, style, GUILayout.Height(25)); //SUBNAME
        levels[selected].font = EditorGUILayout.Popup("Font", levels[selected].font, ToOptions(registry.fonts)); //FONT
        levels[selected].difficulty = (LevelSelect.Level.Difficulty)EditorGUILayout.EnumPopup("Difficulty", levels[selected].difficulty);
        levels[selected].starPosition = EditorGUILayout.Vector2Field("Star", levels[selected].starPosition);
        //levels[selected].prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", levels[selected].prefab, typeof(GameObject), false);
        levels[selected].prefab = EditorGUILayout.IntField("Prefab", levels[selected].prefab);
        GUILayout.EndVertical();
        
        GUI.color = levels[selected].color;
        GUILayout.BeginVertical((GUIStyle)"Box", GUILayout.ExpandHeight(true), GUILayout.Width(200));
        GUI.color = Color.white;
        levels[selected].logo = EditorGUILayout.Popup(levels[selected].logo, ToOptions(registry.logos)); //LOGO
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(registry.logos[levels[selected].logo].texture, GUILayout.ExpandHeight(true));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        levels[selected].color = EditorGUILayout.ColorField(levels[selected].color); //COLOR
        GUILayout.EndVertical();
        
        GUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.Width(100));
        if (GUILayout.Button("Save"))
        {
            CreateFile(levels[selected], filePaths[selected]);
        }
        if (GUILayout.Button("Save As..."))
        {
            CreateFileFromLevel(levels[selected], levels[selected].levelName.ToLower());
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    public string[] ToOptions(Sprite[] logos)
    {
        string[] names = new string[logos.Length];
        for (int i = 0; i < logos.Length; i++)
        {
            names[i] = logos[i].name;
        }
        return names;
    }

    public string[] ToOptions(Font[] fonts)
    {
        string[] names = new string[fonts.Length];
        for (int i = 0; i < fonts.Length; i++)
        {
            names[i] = fonts[i].name;
        }
        return names;
    }

    void DrawMap()
    {
        
    }

    void CreateFileFromLevel(LevelSelect.Level source, string filename)
    {
        string path = EditorUtility.SaveFilePanel("Level", Application.dataPath, filename, "json");
        CreateFile(source, path);
    }

    void CreateFile(LevelSelect.Level source, string path)
    {
        Debug.Log("Saving file: " + path);        

        string file = JsonUtility.ToJson(source, true);        
        System.IO.File.WriteAllText(path, file);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
