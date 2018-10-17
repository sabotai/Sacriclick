using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BBTime : MonoBehaviour {

	float startTime = 0f;
	public bool timeRunning = false;
	//public GameObject[] pauseWhileActive;
	float stopTime = 0f;

	// Use this for initialization
	void Start () {
		startTime = 0f;
		stopTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (timeRunning){
			int min = (int)((Time.time - startTime)/60f);
			int sec = (int)((Time.time - startTime) % 60f);

			string leadM = "";
			string leadS = "";
			if (min < 10) leadM = "0";
			if (sec < 10) leadS = "0";
			GetComponent<Text>().text = leadM + min + ":" + leadS + sec;
			/*
			foreach (GameObject pauseWhile in pauseWhileActive){
				if (pauseWhile.activeSelf) StopTime();
			}
			*/
			if (!BloodMeter.bloodRunning){
				StopTime();
			}
		} else {
			//if (startTime > 0f || GameState.state > 0)	GetComponent<Text>().text = "00:00";
			//else GetComponent<Text>().text = "";
			if (GameState.state != 0){

				int min = (int)((stopTime)/60f);
				int sec = (int)((stopTime) % 60f);

				string leadM = "";
				string leadS = "";
				if (min < 10) leadM = "0";
				if (sec < 10) leadS = "0";
				GetComponent<Text>().text = leadM + min + ":" + leadS + sec;
			}
			/*
			int numActive = 0;
			foreach (GameObject pauseWhile in pauseWhileActive){
				if (pauseWhile.activeSelf) numActive++;
			}
			if (numActive == 0){
				InitTime();
			}
			*/


			if (startTime == 0f || BloodMeter.bloodRunning) InitTime();
		}
	}

	void OnDisable(){
		StopTime();
	}
	void OnEnable(){
		InitTime();
	}
	public void InitTime(){
		if (BloodMeter.bloodRunning || startTime > 0f){
			Debug.Log("start time");
			startTime = Time.time - stopTime;
			timeRunning = true;
		}
	}
	public void StopTime(){
		Debug.Log("stop time");
		stopTime = Time.time - startTime;
		timeRunning = false;
	}
}
