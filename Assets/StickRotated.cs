using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickRotated : MonoBehaviour
{
    public string horizontal = "Horizontal";
    public string vertical = "Vertical";

    void Update ()
    {
        //transform.Rotate(new Vector3(-Input.GetAxis(vertical), -Input.GetAxis(horizontal), 0), Space.World);
        
        Vector3 rot = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        rot += new Vector3(-Input.GetAxis(vertical), -Input.GetAxis(horizontal), 0);
        rot.x = rot.x % 360;
        rot.y = rot.y % 360;
        transform.rotation = Quaternion.Euler(rot);
    }
}
