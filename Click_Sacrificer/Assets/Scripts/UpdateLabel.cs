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
		if (GameState.state == 2) {
			GetComponent<MeshRenderer>().enabled = true;
		} else {

			GetComponent<MeshRenderer>().enabled = false;
		}

		if (GetComponent<SpecialStatus>().specialStat) {
			//if (GetComponent<TextMesh>().text == label) label
			GetComponent<TextMesh>().text = "" + (int)(GetComponent<SpecialStatus>().timeRemaining + 1f);
		} else {
			if (wasSpecial) GetComponent<TextMesh>().text = "#" + myCount;
			
			if (!useNumber) { //calculate the percentage so that ones below the fail theshold are below 0%
				float failThresh = Mathf.Abs(transform.parent.gameObject.GetComponent<Mood>().moodFailThresh);
				float moodRange = failThresh + 1f;

				float consentPct = (GetComponent<ConsentMeter>().consentPct + failThresh) / moodRange; 
				label = (int)(consentPct * 100) + "%";
				GetComponent<TextMesh>().text = label;
			}

		}
		wasSpecial = GetComponent<SpecialStatus>().specialStat;
	}
}
