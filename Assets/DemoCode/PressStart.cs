using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressStart : MonoBehaviour {
    public string NextScene;

    private bool done;

	// Update is called once per frame
	void Update () {
		if(Input.anyKey && !done)
	    {
	        done = true;
	        SceneManager.LoadScene(NextScene);
	    }
	}
}
