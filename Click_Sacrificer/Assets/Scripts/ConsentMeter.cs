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

	// Use this for initialization
	void Start () {
		mommy = transform.parent.gameObject;
		consentPct = mommy.GetComponent<Mood>().mood;
		consentPct += 1f;
		consentPct /= 2f; //convert back to %


		moodDir = mommy.GetComponent<Mood>().moodDir;
		Debug.Log("sibling index = " + transform.GetSiblingIndex());
		if (transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject != null)
			dirLabel = transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (consentPct > 0.5f){
			consentColor = Color.Lerp(Color.yellow, Color.green, consentPct - 0.5f);
		} else {
			consentColor = Color.Lerp(Color.red, Color.yellow, consentPct + 0.5f);			
		}
		GetComponent<TextMesh>().color = consentColor;
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
			if (dir == -1){
				dirLabel.SetActive(true);
				dirLabel.GetComponent<TextMesh>().color = Color.red;
				dirLabel.GetComponent<TextMesh>().text = "▼";
			} else if (dir == 0){
				dirLabel.SetActive(false);
			} else {
				dirLabel.SetActive(true);
				dirLabel.GetComponent<TextMesh>().color = Color.green;
				dirLabel.GetComponent<TextMesh>().text = "▲";
			}
		} else {
			dirLabel.SetActive(false);
		}

	}
}
