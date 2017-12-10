using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLabel : MonoBehaviour {

	public GameObject sacrificer;
	public Transform wayP;
	public string label;
	public int myCount = 0;
	public bool useNumber = false;
	public bool wasSpecial = false;

	// Use this for initialization


	void OnEnable () {		
		if (sacrificer == null) sacrificer = Camera.main.gameObject;
		if (wayP == null) wayP = GameObject.Find("WayParent").transform;
		label = GetComponent<TextMesh>().text;
		myCount = sacrificer.GetComponent<Sacrifice>().sacCount + transform.parent.GetSiblingIndex() + 1;
		
		label = " #" + myCount;//transform.parent.gameObject.GetComponent<Pathfinder>().myCount;
		if (!useNumber) label = "◌";
		GetComponent<TextMesh>().text = label;
	}

	void Start(){
		if (!useNumber) GetComponent<TextMesh>().text = label;
		myCount = sacrificer.GetComponent<Sacrifice>().sacCount + transform.parent.GetSiblingIndex() + 1;

	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<SpecialStatus>().specialStat) {
			//if (GetComponent<TextMesh>().text == label) label
			GetComponent<TextMesh>().text = "" + (int)(GetComponent<SpecialStatus>().timeRemaining + 1f);
		} else {
			if (wasSpecial) GetComponent<TextMesh>().text = "#" + myCount;
			
			if (!useNumber) label = "◌";

		}
		wasSpecial = GetComponent<SpecialStatus>().specialStat;
	}
}
