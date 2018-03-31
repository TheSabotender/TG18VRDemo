using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DebugOverlay : MonoBehaviour
{
    public bool display;


    void OnGUI()
    {
        if (!display)
        {
            return;
        }

        int halfWidth = Screen.width / 2;
        int height = Mathf.Min(Screen.height / 5, 200);

        //LeftStick
        Rect leftGroup = new Rect(0, 0, halfWidth, Screen.height - height);
        GUI.Box(leftGroup, "Left Stick");
        DrawStick("Horizontal", "Vertical", leftGroup);

        //RightStick
        Rect rightGroup = new Rect(halfWidth, 0, halfWidth, Screen.height - height);
        GUI.Box(rightGroup, "Right Stick");
        DrawStick("RightHorizontal", "RightVertical", rightGroup);

        //Other Info        
        GUI.BeginGroup(new Rect(0, Screen.height - height, Screen.width, height), (GUIStyle)"Box");
            DrawInfo();
        GUI.EndGroup();
    }

    void DrawStick(string horizontal, string vertical, Rect container)
    {
        /*
        GUI.Box(new Rect(
                container.x + (container.width * 0.25f),
                container.y + (container.height * 0.25f),
                container.width * 0.5f,
                container.height * 0.5f),
            "");
        */

        float x = container.x + (container.width * 0.5f);
        float y = container.y + (container.height * 0.5f);

        x += (Input.GetAxis(horizontal) * (container.width * 0.25f));
        y += (Input.GetAxis(vertical) * (container.height * 0.25f));

        GUI.Box(new Rect(x, y, 10, 10), "");
    }

    void DrawInfo()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Shift");
        GUILayout.HorizontalSlider(Input.GetAxis("Shift"), 0, 1, GUILayout.Width(100));

        GUILayout.EndHorizontal();
    }
}
