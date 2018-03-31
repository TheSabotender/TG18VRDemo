using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    protected bool hasSetup;

	public virtual IEnumerator Setup ()
	{
	    yield return 0;
	}
    
    public virtual IEnumerator Unload()
    {
        yield return 0;
    }
}
