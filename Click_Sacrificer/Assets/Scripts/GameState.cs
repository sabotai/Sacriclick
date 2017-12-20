using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameState : MonoBehaviour {

	public static int state = 0;
	public int stateRO;
	public GameObject intro;
	public GameObject tipObj;
	bool paused = false;
	public GameObject pauseObj;
	public GameObject tipPanel;
	int prevState;
	PostProcessVolume m_Volume;
    DepthOfField dof;
    float maxAperture = 2.5f;
    float minAperture = 11f;
    float maxFL = 30;
    float minFL = 20;



	// Use this for initialization
	void Start () {
		if (intro == null) intro = GameObject.Find("Intro");
		if (pauseObj == null) pauseObj = GameObject.Find("Pause");
		
		pauseObj.SetActive(false);
		prevState = state; 

		dof = ScriptableObject.CreateInstance<DepthOfField>();
        dof.enabled.Override(true);
        dof.focusDistance.Override(0.1f);
        dof.aperture.Override(2.5f);
        dof.focalLength.Override(minFL);

        m_Volume = PostProcessManager.instance.QuickVolume(Camera.main.gameObject.layer, 100f, dof);
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

		if (Tips.displayingTip) {
			Defocus(2f);
			} else {
				if (!paused) Refocus(2f);
			}

		if (Input.GetKeyDown("escape") && state != 0) {
			if (!paused){
				Pause();
			} else {
				Resume();
				//Refocus();
			}
		}
		stateRO = state;
	}
	public void Defocus(float speed){
		dof.enabled.Override(true);
		//if (dof.aperture > maxAperture) dof.aperture.Override(dof.aperture - Time.deltaTime);
		if (dof.focalLength < maxFL) {
			dof.focalLength.Override(dof.focalLength.value + Time.deltaTime * speed);
			Debug.Log("defocusing... " + dof.focalLength.value);
		}

	}

	public void Refocus(float speed){
		if (dof.focalLength > minFL && dof.enabled.value) {
			dof.focalLength.Override(dof.focalLength.value - Time.deltaTime * speed);
			Debug.Log("refocusing... " + dof.focalLength.value);

		}
			else dof.enabled.Override(false);
			/*
		if (dof.aperture < minAperture) {
			dof.aperture.Override(dof.aperture + Time.deltaTime);
			} else {

				dof.enabled.Override(false);
			}
*/
	}

	public void Pause(){
		paused = true;
		Defocus(2f);
		pauseObj.SetActive(true);
		tipObj.SetActive(false);
		prevState = state;
	}
	public void Resume(){
		paused = false;
		pauseObj.SetActive(false);
		dof.enabled.Override(false);

		//tipObj.SetActive(true);
		state = prevState;
	}

	public void QuitGame (){
		Application.Quit();
	}
}
