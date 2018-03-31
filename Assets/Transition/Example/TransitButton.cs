using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitButton : MonoBehaviour
{
    public Transition Transition;
    public int Destination;
    public float speed = 1;
    public bool invert = false;
    
    void OnGUI()
    {
        if (GUILayout.Button("CHANGE SCENE"))
        {
            Transition t = Instantiate(Transition);
            t.LoadScene(Destination, speed, invert);
        }
    }
}
