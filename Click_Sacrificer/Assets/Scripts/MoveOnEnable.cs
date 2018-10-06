using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnEnable : MonoBehaviour {

	Vector3 origPos;
	public Vector3 initOffset;
	public float speed;
	public float min;
	bool done = false;

	// Use this for initialization
	void Awake () {
		origPos = transform.localPosition;
		transform.localPosition += initOffset;

	}	// Use this for initialization
	void OnEnable () {
		transform.localPosition = origPos + initOffset;
		done = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(transform.localPosition, origPos) > min && !done) {
			transform.localPosition += ((origPos - transform.localPosition) * speed);
		} else {
			done = true;
		}
	}
}
