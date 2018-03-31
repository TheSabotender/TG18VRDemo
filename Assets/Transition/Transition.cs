using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public Material TransitMaterial;

    public void LoadScene(int sceneIndex, float speed = 1, bool invert = false)
    {
        StartCoroutine(Transit(SceneManager.GetSceneByBuildIndex(sceneIndex).name, speed, invert));
    }

    public void LoadScene(string sceneName, float speed = 1, bool invert = false)
    {
        StartCoroutine(Transit(sceneName, speed, invert));
    }

    private IEnumerator Transit(string to, float speed, bool invert)
    {
        RenderTexture Start = GetTextureFromCam(Component.FindObjectOfType<Camera>());
        TransitMaterial.SetFloat("_Amount", 0);
        TransitMaterial.SetFloat("_Invert", invert?1:0);
        TransitMaterial.SetTexture("_WhiteTex", Start);
        TransitMaterial.SetFloat("_Active", 1);
        
        DontDestroyOnLoad(gameObject);
        Scene from = SceneManager.GetActiveScene();
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);        
        yield return SceneManager.UnloadSceneAsync(from);

        RenderTexture Dest = GetTextureFromCam(Component.FindObjectOfType<Camera>());
        TransitMaterial.SetTexture("_BlackTex", Dest);

        float fade = 0;
        while (fade <= 1)
        {
            fade += Time.deltaTime * speed;
            TransitMaterial.SetFloat("_Amount", fade);
            yield return 0;
        }
        TransitMaterial.SetFloat("_Active", 0);
        TransitMaterial.SetTexture("_WhiteTex", null);
        TransitMaterial.SetTexture("_BlackTex", null);

        Start.DiscardContents();
        Dest.DiscardContents();
        Destroy(gameObject);
    }

    private RenderTexture GetTextureFromCam(Camera cam)
    {
        //GL.Clear(true, true, Color.clear);        
        RenderTexture tex = RenderTexture.GetTemporary(Screen.width, Screen.height);
        cam.targetTexture = tex;

        var origCullingMask = cam.cullingMask;
        //var origClearFlags = cam.clearFlags;
        //var origBgColor = cam.backgroundColor;
        //cam.clearFlags = CameraClearFlags.SolidColor;
        //cam.backgroundColor = Color.clear;

        cam.Render();

        cam.cullingMask = origCullingMask;
        //cam.clearFlags = origClearFlags;
        //cam.backgroundColor = origBgColor;
        cam.targetTexture = null;

        return tex;
    }    
}
