using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialLevel : MonoBehaviour {
    [Serializable]
    public class TutorialStage
    {
        [Serializable]
        public class Interactable
        {
            public string inputAxis;
            public float Sensitivity = 0.1f;
            public JoyControlled[] objects;
        }

        public float waitFirst;
        public string text;
        public Interactable[] interactables;
        public bool requiresInverseWorld;

        public float Update()
        {
            float success = 1;
            foreach (Interactable interactable in interactables)
            {
                float i = 1;
                foreach (JoyControlled jc in interactable.objects)
                {
                    jc.Current += Input.GetAxis(interactable.inputAxis) * interactable.Sensitivity;
                    i *= jc.CalculateSuccess();
                }
                success *= i;
            }

            if (requiresInverseWorld)
            {
                success *= FindObjectOfType<TransitionCamera>().fade;
            }

            return success;
        }
    }

    public Text uiText;
    public TutorialStage[] stages;
    public string finalText;
    private int currentStage = -1;

	// Use this for initialization
	void Start ()
	{
	    StartCoroutine(RunTutorial());
	}
	
	IEnumerator RunTutorial ()
	{
	    currentStage++;
	    if (currentStage < stages.Length)
	    {
	        yield return new WaitForSeconds(stages[currentStage].waitFirst);

	        uiText.text = stages[currentStage].text;

	        while (stages[currentStage].Update() < 1f)
	        {
	            yield return 0;
	        }
	        StartCoroutine(RunTutorial());
        }
	    else //end tutorial
        {            
            uiText.text = finalText;
            yield return new WaitForSeconds(0.5f);
            if (!ProfileManager.current.HasUnlock("TutorialComplete") || ProfileManager.current.GetCurrency("LevelsComplete") == 0)
            {
                ProfileManager.current.SetCurrency("LevelsComplete", 1);
                ProfileManager.current.Unlock("TutorialComplete");
            }            
            Loading.Load("LevelSelect");
	    }
	}
}
