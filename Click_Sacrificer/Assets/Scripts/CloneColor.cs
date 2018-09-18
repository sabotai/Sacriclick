using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneColor : MonoBehaviour {

	public GameObject model;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (model)	gameObject.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", model.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor"));
	
	}
}
