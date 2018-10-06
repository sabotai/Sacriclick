using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBTimeout : MonoBehaviour {
	public float timeOut = 5f;
	float startTime = 0f;

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable(){
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Rigidbody>() && Time.time > startTime + timeOut){
			if (GetComponent<Rigidbody>().velocity.sqrMagnitude < 2f ){
				Destroy(GetComponent<Rigidbody>());
				this.enabled = false;
			}
		}
	}
}
