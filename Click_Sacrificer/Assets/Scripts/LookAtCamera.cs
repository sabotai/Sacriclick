using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

	public Transform lookHere;

	// Use this for initialization
	void Start () {
		if (lookHere == null) lookHere = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		//transform.LookAt(lookHere);
		transform.LookAt(Camera.main.transform);
	}
}
