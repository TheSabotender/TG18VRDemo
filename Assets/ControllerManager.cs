using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ControllerManager : MonoBehaviour {

    private static bool playerIndexSet = false;
    private static PlayerIndex playerIndex;
    private static GamePadState state;
    private static GamePadState prevState;

    private static Vector2 TargetRumble;

    public float fadeSpeed = 1;
    
    void OnDestroy()
    {
        GamePad.SetVibration(playerIndex, 0, 0);
    }

    public static void Rumble(float left, float right)
    {
        if (playerIndexSet)
        {
            TargetRumble = new Vector2(Mathf.Max(TargetRumble.x, left), Mathf.Max(TargetRumble.y, right));
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (!playerIndexSet || !prevState.IsConnected)
	    {
	        for (int i = 0; i < 4; ++i)
	        {
	            PlayerIndex testPlayerIndex = (PlayerIndex)i;
	            GamePadState testState = GamePad.GetState(testPlayerIndex);
	            if (testState.IsConnected)
	            {
	                Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
	                playerIndex = testPlayerIndex;
	                playerIndexSet = true;
	            }
	        }
	    }

	    prevState = state;
	    state = GamePad.GetState(playerIndex);

	    GamePad.SetVibration(playerIndex, TargetRumble.x, TargetRumble.y);        
	}

    void LateUpdate()
    {
        TargetRumble = Vector2.MoveTowards(TargetRumble, Vector2.zero, Time.deltaTime * fadeSpeed);
    }
}
