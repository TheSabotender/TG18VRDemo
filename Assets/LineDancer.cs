using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class LineDancer : MonoBehaviour
{
    private Vector3 _end;
    public Vector3 End;

    private int _count;
    public int Count;

    public float offsetPower = 1;

    private LineRenderer lr;

    void Update () {

        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
        }
        
        if (_count != Count || _end != End || Application.isPlaying)
        {
            _end = End;
            _count = Count;

            var angle = Vector3.Angle(Vector3.zero, End);
            lr.positionCount = Count;            
            for (int i = 0; i < lr.positionCount; i++)
            {
                float percentile = (float)i / (lr.positionCount-1f);
                var pos = Vector3.Lerp(Vector3.zero, End, percentile);

                var random = Vector3.zero;
                if (Application.isPlaying)
                {   
                    random = new Vector3
                        (
                            Mathf.Sin(-Time.time + i) * Mathf.Cos(angle),
                            Mathf.Cos(-Time.time + i) * Mathf.Sin(angle),                            
                            0
                        ) * lr.widthCurve.Evaluate(percentile) * offsetPower;
                }

                lr.SetPosition(i, pos + random);                
            }
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + End);
        Gizmos.DrawWireSphere(transform.position + lr.GetPosition(lr.positionCount-1), 10f);
    }
}
