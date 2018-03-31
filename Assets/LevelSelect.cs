using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelect : SceneSetup
{
    [System.Serializable]
    public class Level
    {
        public enum Difficulty { Easy, Normal, Advanced, Expert }

        public string levelName;
        public string subname;
        public Difficulty difficulty;
        public int logo;
        public int font;
        public Color color;
        public Vector2 starPosition;
        public int prefab;

        [NonSerialized] public Button star;
        [NonSerialized] public int LevelIndex;
    }

    public static Level LastSelectedLevel;

    public ScrollRect scroll;
    public Transform levelContainer;
    public GameObject starPrefab;
    [SerializeField]private Level[] _levels;
    public TextAsset[] Levels;

    private Vector2 scrollDestination;
    private Button firstStar;

    void Start()
    {
        if (!Loading.IsSafelyLoaded)
        {
            StartCoroutine(Setup());
        }
    }

    void Update()
    {
        /*
        scroll.horizontalNormalizedPosition = Input.mousePosition.x / Screen.width;
        scroll.verticalNormalizedPosition = Input.mousePosition.y / Screen.height;
        */

        if (firstStar != null && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
        {
            firstStar.Select();
        }

        scroll.horizontalNormalizedPosition = Mathf.Lerp(scroll.horizontalNormalizedPosition, scrollDestination.x, Time.deltaTime);
        scroll.verticalNormalizedPosition = Mathf.Lerp(scroll.verticalNormalizedPosition, scrollDestination.y, Time.deltaTime);

        if (Input.GetButtonDown("Cancel"))
        {
            ControllerManager.Rumble(1, 0);
            Loading.Load("Menu");
        }
        
        /*
        if (Input.GetAxis("Horizontal") > 0)
        {
            ControllerManager.Rumble(1, 0);
            Wait.Then(0.1f, () => { ControllerManager.Rumble(0, 1); });
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            ControllerManager.Rumble(0, 1);
            Wait.Then(0.1f, () => { ControllerManager.Rumble(1, 0); });
        }
        */
    }

    public void OnSelectButton(Transform t)
    {
        //scroll.horizontalNormalizedPosition = t.position.x / ((RectTransform)levelContainer.transform).rect.width;
        //scroll.verticalNormalizedPosition = t.position.y / ((RectTransform)levelContainer.transform).rect.height;

        firstStar = t.GetComponent<Button>();
        scrollDestination = new Vector2(t.position.x / ((RectTransform)levelContainer.transform).rect.width, t.position.y / ((RectTransform)levelContainer.transform).rect.height);
        ControllerManager.Rumble(0,1);
    }

    public override IEnumerator Setup ()
    {
        int levelsComplete = ProfileManager.current.GetCurrency("LevelsComplete");
        if (hasSetup)
        {
            firstStar.Select();
            for (int i = 0; i < _levels.Length; i++)
            {
                _levels[i].star.interactable = levelsComplete >= i;
                yield return 0;
            }
            yield break;
        }

        _levels = new Level[Levels.Length];
        for (var i = 0; i < Levels.Length; i++)
        {
            _levels[i] = JsonUtility.FromJson<Level>(Levels[i].text);            
            _levels[i].star = SpawnStar(_levels[i], _levels[i==0 ? i : i-1].starPosition);
            _levels[i].star.interactable = levelsComplete >= i;
            _levels[i].LevelIndex = i;
            yield return 0;
        }

        if (ProfileManager.current != null)
        {
            firstStar = _levels[Mathf.Clamp(levelsComplete, 0, _levels.Length-1)].star;
        }
        else
        {
            firstStar = _levels[0].star;
        }
        firstStar.Select();

        hasSetup = true;
    }
    
    Button SpawnStar(Level l, Vector2 previousPos)
    {
        GameObject star = Instantiate(starPrefab, levelContainer);
        star.transform.position = l.starPosition;

        Transform lr = star.transform.Find("Line");
        if (l.starPosition == previousPos)
        {
            Destroy(lr.gameObject);
        }
        else
        {
            lr.position = previousPos;
            lr.GetComponent<LineDancer>().End = l.starPosition - previousPos;
        }

        Transform t1 = star.transform.Find("Text");
        t1.GetComponent<Text>().text = l.levelName;
        if (Loading.IsSafelyLoaded)
        {
            t1.GetComponent<Text>().font = AssetRegistry.instance.fonts[l.font];
        }

        Transform t2= star.transform.Find("Text2");
        t2.GetComponent<Text>().text = l.difficulty.ToString();

        if (Loading.IsSafelyLoaded)
        {
            Transform logo = star.transform.Find("Logo");
            logo.GetComponent<Image>().sprite = AssetRegistry.instance.logos[l.logo];
        }

        star.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
        star.GetComponent<Button>().onClick.AddListener(() => { PlayLevel(l);});

        star.SetActive(true);
        
        return star.GetComponent<Button>();
    }

    public void PlayLevel(int index)
    {
        ControllerManager.Rumble(1, 0);
        LastSelectedLevel = _levels[Mathf.Clamp(index, 0, _levels.Length)];
        Loading.Load("Game");
    }

    public void PlayLevel(Level level)
    {
        ControllerManager.Rumble(1, 0);
        LastSelectedLevel = level;
        Loading.Load("Game");
    }

#if UNITY_EDITOR
    [ExposeInEditor]
	void CreateLevelTemplate()
    {
        string file = JsonUtility.ToJson(new Level(), true);

        string path = UnityEditor.EditorUtility.SaveFilePanel("Level Template", Application.dataPath, "template", "json");        
        System.IO.File.WriteAllText(path, file);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
