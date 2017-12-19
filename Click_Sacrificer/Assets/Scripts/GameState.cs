using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public static int state = 0;
	public int stateRO;
	public GameObject intro;
	public GameObject tipObj;
	bool paused = false;
	public GameObject pauseObj;
	public GameObject tipPanel;
	int prevState;

	// Use this for initialization
	void Start () {
		if (intro == null) intro = GameObject.Find("Intro");
		if (pauseObj == null) pauseObj = GameObject.Find("Pause");
		
		pauseObj.SetActive(false);
		prevState = state;
	}
	
	// Update is called once per frame
	void Update () {
		if (intro.activeSelf){
			state = 0;
		} else if (paused){
			state = -1;
		}  else if (Drag.panMode){
			state = 2;
		} else if (CraneGame.beginCraneGame){
			state = 3;
		}else {
			state = 1;
		}


		if (Input.GetKeyDown("escape") && state != 0) {
			if (!paused){
				Pause();
			} else {
				Resume();
			}
		}
		stateRO = state;
	}

	public void Pause(){
		paused = true;

		pauseObj.SetActive(true);
		tipObj.SetActive(false);
		prevState = state;
	}
	public void Resume(){
		paused = false;
		pauseObj.SetActive(false);

		//tipObj.SetActive(true);
		state = prevState;
	}

	public void QuitGame (){
		Application.Quit();
	}
}
