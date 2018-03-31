using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuController : SceneSetup
{
    [System.Serializable]
    public class ParticleGeneration
    {
        public Texture2D particleSource;
        public float threshold = 0.5f;
        public ParticleSystem particleSystem;

        public IEnumerator Generate()
        {
            Debug.Log("Generating galaxy");

            Vector3 offset = new Vector3(particleSource.width * 0.5f, particleSource.height * 0.5f, 0);

            int index = 0;
            List<Vector3> points = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colors = new List<Color>();
            List<int> tris = new List<int>();
            for (int x = 0; x < particleSource.width; x++)
            {
                for (int y = 0; y < particleSource.height; y++)
                {
                    var color = particleSource.GetPixel(x, y);
                    if (color.grayscale > threshold)
                    {
                        points.Add(new Vector3(x, y, 0) - offset);
                        uvs.Add(new Vector2(x / particleSource.width, y / particleSource.height));
                        colors.Add(color);

                        if (index > 3)
                        {
                            tris.Add(index);
                            tris.Add(index - 1);
                            tris.Add(index - 2);
                        }

                        index++;
                        //yield return 0;
                    }
                }
            }

            Debug.Log(points.Count + " points in image");

            Mesh mesh = new Mesh();
            mesh.name = particleSource.name;
            mesh.SetVertices(points);
            mesh.SetUVs(0, uvs);
            mesh.SetColors(colors);
            mesh.SetTriangles(tris.ToArray(), 0);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            yield return new WaitForSeconds(0.01f);

            var shape = particleSystem.shape;
            shape.mesh = mesh;
        }
    }

    public CanvasGroup GameTitle;
    public TransitionCamera camera;
    public ParticleGeneration[] particles;
    public ScrollRect scroll;

    public Button firstButton;
    public TextAsset tutorialLevel;

    private bool started;
    private bool clicked;
    private Vector2 scrollDestination;

    public override IEnumerator Setup ()
    {
        if (hasSetup)
        {
            firstButton.Select();
            yield break;
        }
        
        started = false;  
        GameTitle.alpha = 1;
        clicked = false;

        if (particles != null)
        {
            foreach (ParticleGeneration generation in particles)
            {
                yield return StartCoroutine(generation.Generate());
            }
        }
        
        hasSetup = true;
    }
    
	void Update () {
	    if (!started)
	    {
	        if (Input.anyKey)
	        {
	            started = true;
	            ControllerManager.Rumble(1, 0);
                firstButton.Select();
            }
	        return;
	    }

	    if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
	    {
	        firstButton.Select();
        }

	    //scroll.horizontalNormalizedPosition = Input.mousePosition.x / Screen.width;
	    //scroll.verticalNormalizedPosition = Input.mousePosition.y / Screen.height;
	    scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, scrollDestination.x, Time.deltaTime);
	    scroll.verticalNormalizedPosition = Mathf.Lerp(scroll.verticalNormalizedPosition, scrollDestination.y, Time.deltaTime);

        if (GameTitle.alpha > 0)
	    {
	        GameTitle.alpha -= Time.deltaTime;
	        return;
	    }

	    if (Input.GetButtonUp("Cancel"))
	    {
	        started = false;
	        ControllerManager.Rumble(1, 0);
	        GameTitle.alpha = 1;
        }
    }

    public void OnSelectButton(Transform t)
    {
        firstButton = t.GetComponent<Button>();
        scrollDestination = new Vector2(t.position.x / ((RectTransform)t.parent).rect.width, t.position.y / ((RectTransform)t.parent).rect.height);
        ControllerManager.Rumble(0, 1);
    }

    public void Play()
    {
        if (!started)
            return;

        ControllerManager.Rumble(1, 0);
        if (ProfileManager.current != null && ProfileManager.current.GetCurrency("LevelsComplete") == 0)
        {
            LevelSelect.LastSelectedLevel = JsonUtility.FromJson<LevelSelect.Level>(tutorialLevel.text); ;
            Loading.Load("Scenes/Game");
        }
        else
        {
            Loading.Load("Scenes/LevelSelect");
        }        
    }

    public void About()
    {
        if (!started)
            return;

        ControllerManager.Rumble(1, 0);
        Loading.Load("Scenes/About");
    }

    public void Quit()
    {
        if (!started)
            return;

        Application.Quit();
    }

    [ExposeInEditor]
    void Generate()
    {
        StartCoroutine(Setup());
    }
}
