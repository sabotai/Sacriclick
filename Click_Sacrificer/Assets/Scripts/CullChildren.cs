using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullChildren : MonoBehaviour {

	public int cullThresh = 10;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.childCount > cullThresh){
			Destroy(transform.GetChild(0).gameObject);
		}
	}
}
