﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleCollapse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col){
		if (col.name == "TempleCollapse"){
			GetComponent<ParticleSystem>().Play();
			GetComponent<Rigidbody>().isKinematic = true;
			Debug.Log("trigger smoke");
		}
	}
}
