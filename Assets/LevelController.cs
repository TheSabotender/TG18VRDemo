using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    //public Transform ObjectA;
    //public Transform ObjectB;

    public JoyControlled[] horizontal;
    public JoyControlled[] vertical;
    public JoyControlled[] rightHorizontal;
    public JoyControlled[] rightVertical;

    public float Sensitivity;

    [Range(0, 1)] public float similarity = 0;

    [Range(0, 1)] public float rumblePower = 0;
    public AnimationCurve rumbleCurve;
    [Range(0, 1)] public float rumblePowerB = 0;
    public AnimationCurve rumbleCurveB;

    private bool levelComplete;

    // Use this for initialization
    void Start ()
    {
        levelComplete = false;

        if (horizontal != null)
        {
            foreach (var jc in horizontal)
            {
                jc.Reset();
            }
        }

        if (vertical != null)
        {
            foreach (var jc in vertical)
            {
                jc.Reset();
            }
        }

        if (rightHorizontal != null)
        {
            foreach (var jc in rightHorizontal)
            {
                jc.Reset();
            }
        }

        if (rightVertical != null)
        {
            foreach (var jc in rightVertical)
            {
                jc.Reset();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (!levelComplete)
	    {
	        float hSim = 1;
	        if (horizontal != null)
	        {
	            float i = 1;
	            foreach (var jc in horizontal)
	            {
	                jc.Current += Input.GetAxis("Horizontal") * Sensitivity;
	                i *= jc.CalculateSuccess();
	            }
	            hSim = i;
	        }
	        float vSim = 1;
	        if (vertical != null)
	        {
	            float i = 1;
	            foreach (var jc in vertical)
	            {
	                jc.Current += Input.GetAxis("Vertical") * -Sensitivity;
	                i *= jc.CalculateSuccess();
	            }
	            vSim = i;
	        }
	        float rhSim = 1;
	        if (rightHorizontal != null)
	        {
	            float i = 1;
	            foreach (var jc in rightHorizontal)
	            {
	                jc.Current += Input.GetAxis("RightHorizontal") * Sensitivity;
	                i *= jc.CalculateSuccess();
	            }
	            rhSim = i;
	        }
	        float rvSim = 1;
	        if (rightVertical != null)
	        {
	            float i = 1;
	            foreach (var jc in rightVertical)
	            {
	                jc.Current += Input.GetAxis("RightVertical") * -Sensitivity;
	                i *= jc.CalculateSuccess();
	            }
	            rvSim = i;
	        }

	        similarity = Mathf.Clamp01((hSim + vSim + rhSim + rvSim) / 4f);

	        if (similarity >= 0.999f)
	        {	            
                FinishLevel();
	        }
        }
        
	    if (rumblePower > 0)
	    {
	        var amount = rumbleCurve.Evaluate(similarity * rumblePower);
	        var amountB = rumbleCurveB.Evaluate(similarity * rumblePowerB);
            ControllerManager.Rumble(amount, amountB);
	    }
    }

    void FinishLevel()
    {
        levelComplete = true;
        StartCoroutine(EndLevel());
    }

    IEnumerator EndLevel()
    {
        int currentCurrency = ProfileManager.current.GetCurrency("LevelsComplete");
        if (LevelSelect.LastSelectedLevel.LevelIndex >= currentCurrency)
        {
            ProfileManager.current.SetCurrency("LevelsComplete", currentCurrency + 1);
        }

        yield return new WaitForSeconds(0.5f);
        
        Loading.Load("LevelSelect");
    }
}
