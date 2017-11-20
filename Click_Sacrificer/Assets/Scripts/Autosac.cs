using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autosac : MonoBehaviour {

	public float duration = 5f;
	public int numAutosacs = 0;
	public bool useAutosac = false;
	float startX;
	GameObject diffManager;
	GameObject sacrificer;

	// Use this for initialization
	void Start () {
		startX = Time.time;
		diffManager = GameObject.Find("DifficultyManager");
		sacrificer = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		float interval = duration / numAutosacs;
		if (Time.time > startX + interval && useAutosac){
			if (diffManager.GetComponent<MasterWaypointer>() != null){
				//Debug.Log("found master waypointer");
				if (diffManager.GetComponent<MasterWaypointer>().vicReady){
					
					sacrificer.GetComponent<Sacrifice>().DoSacrifice(sacrificer.GetComponent<Sacrifice>().clickable);
				}
									
			}
			startX = Time.time;
		}
	}
}
