using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCamera : MonoBehaviour
{    
    public Material TransitMaterial;
    public Camera PositiveCamera;
    public Camera NegativeCamera;

    [Range(0, 1)] public float fade;
    public float fadeSpeed = 1;

    public bool allowKeyToggle;

    [Range(0,1)]public float rumble = 0;

    private bool activeWorld = false;
    private RenderTexture otherCamTexture;
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (otherCamTexture == null)
        {
            otherCamTexture = new RenderTexture(Screen.width, Screen.height, 16);
            NegativeCamera.targetTexture = otherCamTexture;
        }

        NegativeCamera.Render();
        
        TransitMaterial.SetTexture("_WhiteTex", src);
        TransitMaterial.SetTexture("_BlackTex", otherCamTexture);

        TransitMaterial.SetFloat("_Active", 1);
        TransitMaterial.SetFloat("_Amount", fade);
        
        Graphics.Blit(src, dest, TransitMaterial);
    }

    void OnDestroy()
    {
        if (otherCamTexture == null)
        {
            Destroy(otherCamTexture);
            NegativeCamera.targetTexture = null;
        }
    }
    
    // Update is called once per frame
    void Update ()
    {        
        if (allowKeyToggle)
        {
            fade = Input.GetAxis("Shift");
        }
        else
        {
            if (Input.GetButtonDown("TapShift"))
            {
                SwitchWorld();
            }
            fade = Mathf.MoveTowards(fade, activeWorld ? 1 : 0, Time.deltaTime * fadeSpeed);            
        }
        
        if (rumble > 0)
        {
            var amount = Mathf.PingPong(fade * 2f, 1f) * rumble;
            ControllerManager.Rumble(amount, amount);
        }
    }

    [ExposeInEditor]
    public void SwitchWorld()
    {
        activeWorld = !activeWorld;
    }
}
