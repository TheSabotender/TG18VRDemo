using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : SceneSetup
{

    public Text Name;
    public Text Subname;
    public Text Difficulty;

    public Image Icon;
    public Image[] ColoredObjects;

    public bool SpawnInstance;
    private GameObject spawn;

    public override IEnumerator Setup () {
	    if (LevelSelect.LastSelectedLevel == null)
	    {
	        yield break;	        
	    }

        if (Name != null)
        {
            Name.text = LevelSelect.LastSelectedLevel.levelName;
            Name.font = AssetRegistry.instance.fonts[LevelSelect.LastSelectedLevel.font];
        }        

        if (Subname != null)
        {
            Subname.text = LevelSelect.LastSelectedLevel.subname;
            Subname.font = AssetRegistry.instance.fonts[LevelSelect.LastSelectedLevel.font];
        }

        if (Difficulty != null)
        {
            Difficulty.text = LevelSelect.LastSelectedLevel.difficulty.ToString();
            Difficulty.font = AssetRegistry.instance.fonts[LevelSelect.LastSelectedLevel.font];
        }

        if (Icon != null)
        {
            Icon.sprite = AssetRegistry.instance.logos[LevelSelect.LastSelectedLevel.logo];
        }

        if(ColoredObjects != null) {
            foreach (Image image in ColoredObjects)
            {
                image.color = LevelSelect.LastSelectedLevel.color;
            }
        }

        if (SpawnInstance)
        {
            spawn = Instantiate(PrefabManager.instance.prefabs[LevelSelect.LastSelectedLevel.prefab], Vector3.zero, Quaternion.identity);
        }
    }

    public override IEnumerator Unload()
    {
        if (spawn != null)
        {
            Destroy(spawn);
            spawn = null;
        }
        yield return 0;
    }

#if UNITY_EDITOR
    [ExposeInEditor]
    void CreateLevelTemplate()
    {
        string file = JsonUtility.ToJson(new LevelSelect.Level(), true);

        string path = UnityEditor.EditorUtility.SaveFilePanel("Level Template", Application.dataPath, "template", "json");        
        System.IO.File.WriteAllText(path, file);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
