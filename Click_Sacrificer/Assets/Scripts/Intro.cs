using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour {
	public GameObject introObj;
	public GameObject[] introHide;
	public GameObject bloodMeter;
	public GameObject helpObj;
	public GameObject tipPanel;

	// Use this for initialization
	void Start () {

		if (PlayerPrefs.GetInt("init") > 0){
			introObj.SetActive(false);
			bloodMeter.SetActive(true);
			if (PlayerPrefs.GetInt("help") == 1) tipPanel.SetActive(true); else tipPanel.SetActive(false);
			if (PlayerPrefs.GetInt("help") == 1) helpObj.SetActive(true);
			if (PlayerPrefs.GetInt("help") == 0) helpObj.SetActive(false);
			PlayerPrefs.SetInt("init", 0); 
			GameObject.Find("Main Camera").GetComponent<CameraMove>().enabled = true;
		} else {
			introObj.SetActive(true);
			for (int i = 0; i < introHide.Length; i++){

			introHide[i].SetActive(false);
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	}
}
