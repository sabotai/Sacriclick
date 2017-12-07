using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialStatus : MonoBehaviour {

	public bool specialStat = false;
	bool oldSS = false;
	float startTime = 0f;
	public float duration = 5f;
	public float timeRemaining = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (specialStat){
			if (specialStat != oldSS) {
				startTime = Time.time;
				transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject.GetComponent<Light>().enabled = true;
			}
			timeRemaining = startTime + duration - Time.time;

			if (timeRemaining < 0f) specialStat = false;

			if (Time.time > startTime + duration) specialStat = false;

		} else {

			transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject.GetComponent<Light>().enabled = false;
		}


		oldSS = specialStat;
	}
}
