using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FractalGroup : MonoBehaviour {

    private int _numberOfArms;
    [Range(1, 100)] public int NumberOfArms;

    private float _extended = 1;
    public float Extended = 1;

    private float _offset = 0;
    [Range(-1, 1)] public float Offset = 0;

    private float _freak = 1;
    public float Freak = 1;

    // Update is called once per frame
    void Update () {
	    if (_numberOfArms != NumberOfArms || _extended != Extended || _offset != Offset || _freak != Freak)
	    {
	        _numberOfArms = NumberOfArms;
	        _extended = Extended;
	        _offset = Offset;
	        _freak = Freak;

	        Generate();
	    }
    }

    [ExposeInEditor]
    void Generate()
    {
        foreach (var child in GetComponentsInChildren<fractal>())
        {
            child.NumberOfArms = NumberOfArms;
            child.Extended = Extended;
            child.Offset = Offset;
            child.Freak = Freak;

            child.Generate();
        }
    }
}
