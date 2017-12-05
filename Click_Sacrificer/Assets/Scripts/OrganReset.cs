using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganReset : MonoBehaviour {

	Vector3 origin;
	// Use this for initialization
	void Awake () {
		origin = transform.position;
	}
	void OnEnable(){
		transform.position = origin;
	}

	// Update is called once per frame
	void Update () {
		}
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Temple"){
			Camera.main.gameObject.GetComponent<CraneGame>().beginCraneGame = false;

		}
	}
	
}
