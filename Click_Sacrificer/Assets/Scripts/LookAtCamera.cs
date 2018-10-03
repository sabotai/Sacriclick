using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

	public Transform lookHere;
	public bool lockReadyVics = true;
	Transform lookPip;


	// Use this for initialization
	void Start () {
		if (lookHere == null) lookHere = Camera.main.transform;
		if (lockReadyVics && GameObject.Find("PiP Camera")) lookPip = GameObject.Find("PiP Camera").transform;
	}
	
	// Update is called once per frame
	void Update () {
		//transform.LookAt(lookHere);
		if (lockReadyVics){
			if (transform.parent.GetSiblingIndex() > 5){

				transform.LookAt(Camera.main.transform.position);

			} else {

				transform.LookAt(lookPip);

			}
		} else {
			transform.LookAt(Camera.main.transform.position);

		}
	}
}
