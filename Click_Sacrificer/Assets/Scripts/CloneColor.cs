using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneColor : MonoBehaviour {

	public GameObject model;
	public bool setEmission = true;
	public bool setTint = false;
	public bool customA = false;
	public float aAmount = 0.25f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (model)	{
			Color modelColor = model.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
			if (customA) modelColor = new Color(modelColor.r, modelColor.g, modelColor.b, aAmount);
			if (setEmission)	gameObject.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", modelColor);
			if (setTint)	gameObject.GetComponent<MeshRenderer>().material.SetColor ("_TintColor", modelColor);

		}
	
	}
}
