using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLabel : MonoBehaviour {

	public GameObject sacrificer;
	public Transform wayP;
	string label;
	public int myCount = 0;
	public bool useNumber = false;
	// Use this for initialization


	void OnEnable () {		
		if (sacrificer == null) sacrificer = GameObject.Find("Main Camera");
		if (wayP == null) wayP = GameObject.Find("WayParent").transform;
		label = GetComponent<TextMesh>().text;
		myCount = sacrificer.GetComponent<Sacrifice>().sacCount + transform.parent.GetSiblingIndex() + 1;
		
		label = " #" + myCount;//transform.parent.gameObject.GetComponent<Pathfinder>().myCount;
		if (!useNumber) label = "◌";
		GetComponent<TextMesh>().text = label;
	}

	void Start(){
		if (!useNumber) GetComponent<TextMesh>().text = label;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
