using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class borgMovement : MonoBehaviour {
	public float maxDrift = 50.0f;

	private Vector3 targetPosition;
	private int timer = 0;

	// Use this for initialization
	void Start () {

		this.targetPosition.x = 0;
		this.targetPosition.y = 0;
		this.targetPosition.z = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		this.timer++;

		float timeIntersecter = this.timer % 40;

	
		if (timeIntersecter == 0 ) {
			this.targetPosition.x = Random.Range(-maxDrift, maxDrift);
			this.targetPosition.y = Random.Range(-maxDrift, maxDrift);
		}

		float step = 4 * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

	}
}
