using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BForBack : MonoBehaviour
{

    public string Level = "Menu";
    public ScrollRect scroll;
    [Range(0,1)]public float rumble;

    // Update is called once per frame
    void Update () {
	    if (Input.GetButtonDown("Cancel"))
	    {
	        ControllerManager.Rumble(1, 0);
            Loading.Load(Level);
	    }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (scroll != null)
        {
            scroll.horizontalNormalizedPosition = (h + 1) * 0.5f;
            scroll.verticalNormalizedPosition = (v + 1) * -0.5f;
            
        }

        if (rumble > 0)
        {
            var amount = Vector2.Distance(Vector2.zero, new Vector2(h, v)) * rumble;
            ControllerManager.Rumble(amount, amount);
        }
    }
}
