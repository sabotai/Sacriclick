using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLabel : MonoBehaviour {

	public GameObject sacrificer;
	public Transform wayP;
	string label;
	// Use this for initialization
	void Start () {		
		if (sacrificer == null) sacrificer = GameObject.Find("Main Camera");
		if (wayP == null) wayP = GameObject.Find("WayParent").transform;
		label = GetComponent<TextMesh>().text;
	}
	
	// Update is called once per frame
	void Update () {
		label = "#" + (sacrificer.GetComponent<Sacrifice>().sacCount + transform.parent.gameObject.GetComponent<Pathfinder>().currentWaypoint);
		label = "#" + transform.parent.gameObject.GetComponent<Pathfinder>().myCount;
		GetComponent<TextMesh>().text = label;
	}
}
