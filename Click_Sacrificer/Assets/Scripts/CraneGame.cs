﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CraneGame : MonoBehaviour {

	public bool beginCraneGame = false;
	float craneGameZoom = 42.5f;
	public Transform craneCam;
	Transform newStartFocus;
	Transform origEndFocus,	origStartFocus;
	float origEndZoomAmt, origStartZoomAmt;
	public GameObject craneParent;
	public GameObject claw;
	bool ready = false;
	public GameObject vics, basket;
	public float transitionSpeed = .05f;
	public float timeOutDuration = 3f;
	float startTime = -1f;

	public float craneGameEE = 1f;
	public float craneGameSD = 1f;
	float origCraneGameEE = .25f;
	float origCraneGameSD = .6f;


	// Use this for initialization
	void Start () {
		newStartFocus = GameObject.Find("perspective-storage").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (beginCraneGame){


			if (!craneParent.active){
				startTime = -1f;
				//move/rotate camera

				//store original settings
				origEndFocus = GetComponent<CameraMove>().endFocus;
				origStartFocus = GetComponent<CameraMove>().startFocus;
				origEndZoomAmt = GetComponent<CameraMove>().endZoomAmt;
				origStartZoomAmt = GetComponent<CameraMove>().startZoomAmt;

				newStartFocus.position = Camera.main.transform.position;
				newStartFocus.rotation = Camera.main.transform.rotation;
				newStartFocus.localScale = Camera.main.transform.localScale;
				GetComponent<CameraMove>().startFocus = newStartFocus;

				GetComponent<CameraMove>().endFocus = craneCam;
				GetComponent<CameraMove>().endZoomAmt = craneGameZoom;
				GetComponent<CameraMove>().startZoomAmt = Camera.main.fieldOfView;

				//enable crane stuff
				craneParent.SetActive(true);

				Camera.main.transform.SetParent(craneCam.parent, false);
				//store the originals before we change
				origCraneGameEE = Camera.main.gameObject.GetComponent<EdgeDetection>().edgeExp;
				origCraneGameSD = Camera.main.gameObject.GetComponent<EdgeDetection>().sampleDist;
				//claw.transform.SetParent(Camera.main.transform, false);
				GetComponent<CameraMove>().zoom = 0f;
				GetComponent<CameraMove>().oldZoom = 0f;

				//make initial assignments based on new hierarchy
				//claw.GetComponent<Claw>().Start();
			}


			if (!ready){
				//keep pushing
				GetComponent<CameraMove>().forceAmt += transitionSpeed;

				if (GetComponent<CameraMove>().zoom >= 1f && GetComponent<CameraMove>().oldZoom >= 1f){

					Camera.main.gameObject.GetComponent<EdgeDetection>().edgeExp = craneGameEE;
					Camera.main.gameObject.GetComponent<EdgeDetection>().sampleDist = craneGameSD;
					GetComponent<CameraMove>().forceAmt = 1f;
					Camera.main.transform.SetParent(craneCam.parent, false);
					claw.transform.SetParent(Camera.main.transform, false);
					claw.GetComponent<Claw>().Start();
					ready = true;
					GetComponent<CameraMove>().enabled = false;
					basket.SetActive(true);
					vics.SetActive(false);
				} 

			} else{ //ready = true

				if (claw.GetComponent<Claw>().completed && startTime < 0f){
					startTime = Time.time;
				}

				bool timeOut = false;
				if (startTime > 0f && Time.time > startTime + timeOutDuration) {
					timeOut = true;
				}
				//reset to regular game
				if (Input.GetKeyDown(KeyCode.F11)) {
					Debug.Log("crane game exit bc F11");
					beginCraneGame = false;
					startTime = -1f;
				}
				if (timeOut) {
					Debug.Log("crane game exit bc timeout");
					beginCraneGame = false;
					startTime = -1f;
				}
				if (claw.GetComponent<Claw>().craneRotPct > 0f && claw.GetComponent<Claw>().craneRotPct < 0.2f && !claw.GetComponent<Claw>().grabbing){
					
					Debug.Log("crane game exit bc grab fail");
					beginCraneGame = false;
					startTime = -1f;
				} 
			}

		} else { //if !beginCraneGame
			//assuming you are coming from the crane game...
			//reset all the camera stuff and items
			if (craneParent.active){
				ready = false;
				vics.SetActive(true);

				Camera.main.gameObject.GetComponent<EdgeDetection>().edgeExp = origCraneGameEE;
				Camera.main.gameObject.GetComponent<EdgeDetection>().sampleDist = origCraneGameSD;
				GetComponent<CameraMove>().enabled = true;
				GetComponent<CameraMove>().startFocus = origStartFocus;
				GetComponent<CameraMove>().endFocus = origEndFocus;
				GetComponent<CameraMove>().endZoomAmt = origEndZoomAmt;
				GetComponent<CameraMove>().startZoomAmt = origStartZoomAmt;
				GetComponent<CameraMove>().forceAmt = 0f;
				claw.transform.SetParent(craneParent.transform.GetChild(0), false);
				Camera.main.transform.SetParent(null, false);
				basket.SetActive(false);
				craneParent.SetActive(false);
			}
		}

		if (Input.GetKeyDown(KeyCode.F10) && !beginCraneGame) beginCraneGame = true;


	}

	//currently kills all the available vics
	public IEnumerator winCraneGame(){
		GameObject diffManager = GameObject.Find("DifficultyManager");
		GameObject[] vics = diffManager.GetComponent<MasterWaypointer>().movables;

		Debug.Log("Win dat crane game... kill " + vics.Length);
		int howManySacced = 0;
		Camera.main.gameObject.GetComponent<Sacrifice>().easyMode = true; //use easy mode so they arent penalized for mood
		while (howManySacced < vics.Length){
			Debug.Log("trying to sac " + howManySacced);
			if (diffManager.GetComponent<MasterWaypointer>().vicReady){					
				Camera.main.gameObject.GetComponent<Sacrifice>().DoSacrifice(Camera.main.gameObject.GetComponent<Sacrifice>().clickable);
				howManySacced++;	
			}

		}
		Camera.main.gameObject.GetComponent<Sacrifice>().easyMode = false;

		yield return null;
	}

}
