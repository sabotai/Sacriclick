using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFreedom : MonoBehaviour {

	public GameObject parentObj;
	Rigidbody parentRig;
	// Use this for initialization
	void Start () {
		parentRig = parentObj.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.parent){
			if (parentObj.GetComponent<Mood>() == null) {
				transform.parent = null;
				gameObject.AddComponent<Rigidbody>();
				gameObject.GetComponent<CapsuleCollider>().enabled = true;
			}
		}
	}
}
