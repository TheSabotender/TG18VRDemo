using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    private static Loading instance;
    public static bool IsSafelyLoaded { get; private set; }
    private Dictionary<string, Scene> loadedScenes;

    public string startingScene;
    public RawImage LoadImage;
    public Texture2D blackImage;

    public float rotateSpeed = 1;
    public float fadeSpeed = 1;

    public GameObject[] assetsToLoad;

    private RenderTexture a, b;
    private float loadVisible;

	void Start ()
	{
	    loadVisible = 1;
	    UpdateLoadingTexture();
        instance = this;
	    IsSafelyLoaded = false;

        DontDestroyOnLoad(gameObject);

        loadedScenes = new Dictionary<string, Scene>();
	    SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
	    StartCoroutine(DoFirstLoad());
    }

    private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        string name = arg0.path.Replace("Assets/", "").Replace(".unity", "");
        loadedScenes.Add(name, arg0);
        Debug.Log("New scene loaded: '" + name + "', added to list.");
    }

    private void UpdateLoadingTexture()
    {
        /*
        if (otherCamTexture == null)
        {
            otherCamTexture = new RenderTexture(Screen.width, Screen.height, 16);
            NegativeCamera.targetTexture = otherCamTexture;
        }

        NegativeCamera.Render();
        */
        Material m = LoadImage.material;
        if (a != null)
        {
            m.SetTexture("_WhiteTex", a);
        }
        else
        {
            m.SetTexture("_WhiteTex", blackImage);
        }

        m.SetTexture("_BlackTex", b);

        m.SetFloat("_Amount", loadVisible);
        LoadImage.material = m;

        //Graphics.Blit(src, dest, TransitMaterial);
    }
    
    void OnDestroy()
    {
        LoadImage.material.SetFloat("_Amount", 1);
    }
    
    private IEnumerator DoFirstLoad()
    {;
        //LOAD INITIAL STUFF
        Debug.Log("Loading");
        foreach (GameObject o in assetsToLoad)
        {
            DontDestroyOnLoad(Instantiate(o));
        }
        
        if (b == null)
        {
            b = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 16);
        }

        loadVisible = 0;
        UpdateLoadingTexture();

        LoadImage.gameObject.SetActive(true);

        //LOAD NEW SCENE        
        if (!loadedScenes.ContainsKey(startingScene))
        {
            Debug.Log("Loading Scene: " + startingScene);
            SceneManager.LoadSceneAsync(startingScene, LoadSceneMode.Single);
        }
        while (!loadedScenes.ContainsKey(startingScene))
        {
            yield return 0;
        }

        //ENABLE NEW SCENE
        Debug.Log("Enabling scene: " + startingScene);
        SceneManager.SetActiveScene(loadedScenes[startingScene]);
        Scene newScene = SceneManager.GetActiveScene();
        SceneSetup setupObject = null;
        foreach (GameObject rootGameObject in newScene.GetRootGameObjects())
        {
            rootGameObject.SetActive(true);
            if (rootGameObject.GetComponent<SceneSetup>() != null)
            {
                setupObject = rootGameObject.GetComponent<SceneSetup>();
            }

            if (rootGameObject.GetComponent<Camera>())
            {
                rootGameObject.GetComponent<Camera>().targetTexture = b;
            }
        }

        if (setupObject != null)
        {
            Debug.Log("Found scene Startup code");
            yield return setupObject.Setup();
        }

        //TRANSITION SCENE
        LoadImage.material.SetFloat("_Invert", 0);
        while (loadVisible <= 1)
        {
            loadVisible += Time.deltaTime * fadeSpeed;
            //loader.alpha += Time.deltaTime * fadeSpeed;
            yield return 0;
        }

        foreach (GameObject rootGameObject in newScene.GetRootGameObjects())
        {
            if (rootGameObject.GetComponent<Camera>())
            {
                rootGameObject.GetComponent<Camera>().targetTexture = null;
            }
        }
        LoadImage.gameObject.SetActive(false);
        IsSafelyLoaded = true;
        Debug.Log("Game ready");
    }

    public static void Load(string sceneName)
    {
        if (sceneName.StartsWith("Scenes/"))
        {
            instance.StartCoroutine(instance.DoLoad(sceneName));
        }
        else
        {
            instance.StartCoroutine(instance.DoLoad("Scenes/" + sceneName));
        }
    }

    private IEnumerator DoLoad(string sceneName)
    {
        if (a == null)
        {
            a = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 16);
        }
        if (b == null)
        {
            b = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 16);
        }
        Scene originalScene = SceneManager.GetActiveScene();
        foreach (GameObject o in originalScene.GetRootGameObjects())
        {
            if (o.GetComponent<Camera>())
            {
                o.GetComponent<Camera>().targetTexture = a;
            }
        }

        loadVisible = 0;
        UpdateLoadingTexture();

        LoadImage.gameObject.SetActive(true);

        //LOAD NEW SCENE        
        if (!loadedScenes.ContainsKey(sceneName))
        {
            Debug.Log("Loading Scene: " + startingScene);
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        while (!loadedScenes.ContainsKey(sceneName))
        {
            yield return 0;
        }

        //ENABLE NEW SCENE
        Debug.Log("Enabling scene: " + startingScene);
        SceneManager.SetActiveScene(loadedScenes[sceneName]);
        Scene newScene = SceneManager.GetActiveScene();
        SceneSetup setupObject = null;
        foreach (GameObject rootGameObject in newScene.GetRootGameObjects())
        {
            rootGameObject.SetActive(true);
            if (rootGameObject.GetComponent<SceneSetup>() != null)
            {
                setupObject = rootGameObject.GetComponent<SceneSetup>();
            }

            if (rootGameObject.GetComponent<Camera>())
            {
                rootGameObject.GetComponent<Camera>().targetTexture = b;
            }
        }

        if (setupObject != null)
        {
            Debug.Log("Found scene Startup code");
            yield return setupObject.Setup();
        }

        //TRANSITION SCENE
        LoadImage.material.SetFloat("_Invert", 0);
        while (loadVisible <= 1)
        {
            loadVisible += Time.deltaTime * fadeSpeed;
            //loader.alpha += Time.deltaTime * fadeSpeed;
            yield return 0;
        }

        //HIDE OLD SCENE
        SceneSetup setup = null;
        foreach (GameObject rootGameObject in originalScene.GetRootGameObjects())
        {
            if (rootGameObject.GetComponent<SceneSetup>() != null)
            {
                setup = rootGameObject.GetComponent<SceneSetup>();
            }
            else
            {
                rootGameObject.SetActive(false);
            }
        }
        if (setup != null)
        {
            yield return StartCoroutine(setup.Unload());
            setup.gameObject.SetActive(false);
        }

        //FADE OUT LOADER
        //Debug.Log("Fading out loading");
        //LoadImage.material.SetFloat("_Invert", 1);
        //while (loadVisible >= 0)
        //{
        //    loadVisible -= Time.deltaTime * fadeSpeed;            
        //loader.alpha -= Time.deltaTime * fadeSpeed;
        //    yield return 0;
        //}

        foreach (GameObject rootGameObject in newScene.GetRootGameObjects())
        {
            if (rootGameObject.GetComponent<Camera>())
            {
                rootGameObject.GetComponent<Camera>().targetTexture = null;
            }
        }
        LoadImage.gameObject.SetActive(false);
    }
}
