using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyControlled : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float start;

    [SerializeField] private Vector3 minPosition;
    [SerializeField] private Quaternion minRotation;
    [SerializeField] private Vector3 minScale = Vector3.one;

    [SerializeField] private Vector3 maxPosition;
    [SerializeField] private Quaternion maxRotation;
    [SerializeField] private Vector3 maxScale = Vector3.one;

    [SerializeField] private Vector3 destPosition;
    [SerializeField] private Quaternion destRotation;
    [SerializeField] private Vector3 destScale = Vector3.one;
    
    [SerializeField][Range(0,1)]private float current;
    public virtual float Current {
        get { return current; } set
        {
            if (Mathf.Clamp01(value) == current)
                return;

            current = Mathf.Clamp01(value);
            UpdateValues();        
        }        
    }    

    [SerializeField][Range(0,1)]private float success;

    [ExposeInEditor]
    public void UpdateValues()
    {
        transform.localPosition = Vector3.Lerp(minPosition, maxPosition, current);
        transform.localRotation = Quaternion.Lerp(minRotation, maxRotation, current);
        transform.localScale = Vector3.Lerp(minScale, maxScale, current);

        CalculateSuccess();
    }

    public virtual float CalculateSuccess()
    {
        float maxDistP = Vector3.Distance(minPosition, maxPosition);
        float maxDistR = Quaternion.Angle(minRotation, maxRotation);
        float maxDistS = Vector3.Distance(minScale, maxScale);

        float pos = 1;
        if (maxDistP > 0)
        {
            pos = 1 - (Vector3.Distance(transform.localPosition, destPosition) / maxDistP);
        }

        float rot = 1;
        if (maxDistR > 0)
        {
            rot = 1 - (Quaternion.Angle(transform.localRotation, destRotation) / maxDistR);
        }

        float scl = 1;
        if (maxDistS > 0)
        {
            scl = 1 - (Vector3.Distance(transform.localScale, destScale) / maxDistS);
        }

        //success = Mathf.Abs(pos);
        //success = Mathf.Abs(rot);
        //success = Mathf.Abs(scl);
        success = Mathf.Abs(pos) * Mathf.Abs(rot) * Mathf.Abs(scl);

        return success;
    }

    [ExposeInEditor]
    private void SetMin()
    {
        minPosition = transform.localPosition;
        minRotation = transform.localRotation;
        minScale = transform.localScale;
    }
    [ExposeInEditor]
    private void SetMax()
    {
        maxPosition = transform.localPosition;
        maxRotation = transform.localRotation;
        maxScale = transform.localScale;
    }
    [ExposeInEditor]
    private void SetDest()
    {
        destPosition = transform.localPosition;
        destRotation = transform.localRotation;
        destScale = transform.localScale;
    }

    [ExposeInEditor]
    public virtual void Reset()
    {        
        success = 0;
        current = start;

        Current = start;
        
        CalculateSuccess();
        UpdateValues();
    }

}
