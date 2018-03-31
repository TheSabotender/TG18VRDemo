using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    public GameObject[] prefabs;

	// Use this for initialization
	void Start ()
	{
	    instance = this;
	}
	
}
