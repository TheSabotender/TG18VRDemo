using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class fractal : MonoBehaviour
{

    private int _numberOfArms;
    [Range(1, 100)] public int NumberOfArms;

    private float _extended = 1;
    public float Extended = 1;

    private float _offset = 0;
    [Range(-1,1)]public float Offset = 0;

    private float _freak = 1;
    public float Freak = 1;

    void Update()
    {
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
    public void Generate()
    {
        float itteration = 360f / NumberOfArms;
        LineRenderer lr = GetComponent<LineRenderer>();
        List<Vector3> points = new List<Vector3>();

        for(int i = 0; i < NumberOfArms; i++)
        {            
            float sa = itteration * i;
            float se = itteration * (i+1f);

            Vector3 s = new Vector3(Mathf.Cos(Mathf.Deg2Rad * sa), Mathf.Sin(Mathf.Deg2Rad * sa));
            Vector3 e = new Vector3(Mathf.Cos(Mathf.Deg2Rad * se), Mathf.Sin(Mathf.Deg2Rad * se));

            points.AddRange(GenerateArm(s, e));
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        lr.loop = true;
    }

    Vector3[] GenerateArm(Vector3 start, Vector3 end)
    {
        List<Vector3> list = new List<Vector3>();
        float angle = Vector3.Angle(Vector3.zero, Vector3.Lerp(start, end, 0.5f));

        list.Add(start);
        //list.Add(Vector3.Lerp(start,end,0.5f) + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)));
        list.Add(Vector3.Lerp(start,end,0.5f + (Offset*0.5f)) * Extended);
        if (Freak != 1)
        {
            list.Add(Vector3.Lerp(start, end, 0.5f + (Offset * 0.5f)) * Extended * Freak);
        }        
        //list.Add(end);

        return list.ToArray();
    }
}
