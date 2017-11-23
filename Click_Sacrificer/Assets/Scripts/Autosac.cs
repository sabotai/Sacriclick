﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autosac : MonoBehaviour {

	public float duration = 10f;
	public int numAutosacs = 0;
	public bool useAutosac = false;
	public int numClicks = 5;
	public int clicksRemaining = 0;
	float startX;
	GameObject diffManager;
	GameObject sacrificer;
	AudioSource audsrc;
	GameObject spawn;
	public AudioClip autoExhaustSnd;
	public Material autosacMat;

	// Use this for initialization
	void Start () {
		startX = Time.time;
		diffManager = GameObject.Find("DifficultyManager");
		sacrificer = GameObject.Find("Main Camera");
		spawn = GameObject.Find("autosacSpawn");
		audsrc = spawn.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (sacrificer.GetComponent<Sacrifice>().failed) numAutosacs = 0;

		float interval = duration / numAutosacs;
		if (Time.time > startX + interval && useAutosac && numAutosacs > 0){
			if (diffManager.GetComponent<MasterWaypointer>() != null){
				//Debug.Log("found master waypointer");
				if (diffManager.GetComponent<MasterWaypointer>().vicReady){					
					sacrificer.GetComponent<Sacrifice>().DoSacrifice(sacrificer.GetComponent<Sacrifice>().clickable);
					clicksRemaining--;
				}				
			}
			startX = Time.time;
			int potentialTotal = numAutosacs * numClicks;
			int lastAutoJuice = potentialTotal - clicksRemaining;
			Debug.Log("auto: " + lastAutoJuice);
			if (lastAutoJuice >= numClicks){ //youve expended the last autosac in the series
				expendAuto();
			} else { //display the color
				Color lastColor = autosacMat.color;
				lastColor.a = 0.8f - ((float)lastAutoJuice / (float)numClicks);
				spawn.transform.GetChild(spawn.transform.childCount - 1).gameObject.GetComponent<MeshRenderer>().material.color = lastColor;
			}
		}

	}

	void expendAuto(){
		audsrc.PlayOneShot(autoExhaustSnd);
		Destroy(spawn.transform.GetChild(spawn.transform.childCount - 1).gameObject);
		numAutosacs--;
		sacrificer.GetComponent<BloodMeter>().autosacNumber = numAutosacs;
	}
}
