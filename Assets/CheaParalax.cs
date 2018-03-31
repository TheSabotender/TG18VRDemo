using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheaParalax : MonoBehaviour
{
    public Transform source;

    public float Multiplier;

    void Update ()
    {
        transform.position = new Vector3(source.position.x * Multiplier, source.position.y * Multiplier, transform.position.z);
    }
}
