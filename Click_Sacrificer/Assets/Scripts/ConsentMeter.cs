using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsentMeter : MonoBehaviour {
	Color consentColor;
	public float consentPct;
	public float decreaseAmt;
	GameObject mommy;
	public float moodDirDispThresh = 0.3f;
	float moodDir;
	GameObject dirLabel;
	public Color totalConsentColor;
	//public Color totalConsentColorCB;
	public Color nonconsentColor = Color.red;
	//public Color nonconsentColorCB;
	Color midColor;

	// Use this for initialization
	void Start () {
		mommy = transform.parent.gameObject;
		consentPct = mommy.GetComponent<Mood>().mood;
		consentPct += 1f;
		consentPct /= 2f; //convert back to %


		moodDir = mommy.GetComponent<Mood>().moodDir;
		//Debug.Log("sibling index = " + transform.GetSiblingIndex());
		if (transform.GetChild(0).gameObject != null)
			dirLabel = transform.GetChild(0).gameObject;

		midColor = Color.yellow;//Color.Lerp(nonconsentColor, totalConsentColor, 0.5f);

		if (ColorblindMode.cbMode){
			totalConsentColor = ColorblindMode.cbGreen;
			nonconsentColor = ColorblindMode.cbRed;
			midColor = Color.white;
		}
	}

	
	// Update is called once per frame
	void Update () {
		///*
		if (ColorblindMode.cbMode){
			totalConsentColor = ColorblindMode.cbGreen;
			nonconsentColor = ColorblindMode.cbRed;
			midColor = Color.white;
		}
		//*/

		if (consentPct > 0.5f){
			consentColor = Color.Lerp(midColor, totalConsentColor, consentPct - 0.5f);
		} else {
			consentColor = Color.Lerp(nonconsentColor, midColor, consentPct + 0.5f);			
		}
		GetComponent<TextMesh>().color = consentColor;

			transform.parent.gameObject.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", consentColor);
			transform.parent.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", consentColor);

		//make sure the person is colored the same
		foreach (Material mat in transform.parent.gameObject.GetComponent<MeshRenderer>().materials){
			mat.SetColor ("_EmissionColor", consentColor);
			//transform.parent.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", consentColor);
			
		}
		//consentPct -= decreaseAmt * Time.deltaTime;
		consentPct = mommy.GetComponent<Mood>().mood;
		moodDir = mommy.GetComponent<Mood>().moodDir;
	

			if (moodDir > moodDirDispThresh){
				displayMoodDir(1);
			} else if (moodDir < -moodDirDispThresh){
				displayMoodDir(-1);
			} else {
				displayMoodDir(0);
			}

	
	}


	void displayMoodDir(int dir){
		
		if (consentPct < 1f - moodDirDispThresh){
			dirLabel.GetComponent<TextMesh>().fontSize = 100;
			if (dir == -1){
				dirLabel.SetActive(true);
				dirLabel.GetComponent<TextMesh>().color = nonconsentColor;
				dirLabel.GetComponent<TextMesh>().text = "▼";
			} else if (dir == 0){
				dirLabel.SetActive(false);
			} else {
				dirLabel.SetActive(true);
				dirLabel.GetComponent<TextMesh>().color = consentColor;
				dirLabel.GetComponent<TextMesh>().text = "▲";
			}

			float moodLvl = transform.parent.gameObject.GetComponent<Mood>().moodLevel;
			if (moodLvl > 2f){

				dirLabel.SetActive(true);
				dirLabel.GetComponent<TextMesh>().color = nonconsentColor;
				dirLabel.GetComponent<TextMesh>().fontSize = 150;
				dirLabel.GetComponent<TextMesh>().text = "!!!";
			} else if (moodLvl == 2f){

				dirLabel.SetActive(true);
				dirLabel.GetComponent<TextMesh>().color = nonconsentColor;
				dirLabel.GetComponent<TextMesh>().fontSize = 130;
				dirLabel.GetComponent<TextMesh>().text = "!!";
			} else if (moodLvl == 1f){

				dirLabel.SetActive(true);
				dirLabel.GetComponent<TextMesh>().color = nonconsentColor;
				dirLabel.GetComponent<TextMesh>().fontSize = 100;
				dirLabel.GetComponent<TextMesh>().text = "!";
			}
		} else {
			dirLabel.SetActive(false);
		}


	}
}
