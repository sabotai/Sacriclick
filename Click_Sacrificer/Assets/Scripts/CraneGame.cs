using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneGame : MonoBehaviour {

	public bool beginCraneGame = false;
	float craneGameZoom = 42.5f;
	public Transform craneCam;
	Transform origEndFocus, origStartFocus;
	float origEndZoomAmt;
	public GameObject craneParent;
	public GameObject claw;
	bool ready = false;
	public GameObject vics, basket;
	public float transitionSpeed = .05f;
	// Use this for initialization
	void Start () {
		origEndFocus = GetComponent<CameraMove>().endFocus;
		origStartFocus = GetComponent<CameraMove>().startFocus;
		origEndZoomAmt = GetComponent<CameraMove>().endZoomAmt;
		//claw = GameObject.Find("claw-2");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (beginCraneGame){
			if (!craneParent.active){
				//move/rotate camera

				//force the front perspective
				GetComponent<CameraMove>().startFocus = Camera.main.transform;
				GetComponent<CameraMove>().endFocus = craneCam;
				GetComponent<CameraMove>().endZoomAmt = craneGameZoom;
				//if (GetComponent<CameraMove>().forceAmt >= 1f) GetComponent<CameraMove>().startFocus = craneCam;


				//enable crane stuff
				craneParent.SetActive(true);

				Camera.main.transform.SetParent(craneCam.parent, false);
				//claw.transform.SetParent(Camera.main.transform, false);
				GetComponent<CameraMove>().forceAmt = 0f;
				claw.GetComponent<Claw>().Start();
			}
			if (!ready){
				GetComponent<CameraMove>().forceAmt += transitionSpeed;
				if (GetComponent<CameraMove>().forceAmt >= .99f){
					GetComponent<CameraMove>().forceAmt = 1f;
					Camera.main.transform.SetParent(craneCam.parent, false);
					claw.transform.SetParent(Camera.main.transform, false);
					claw.GetComponent<Claw>().Start();
					ready = true;
					GetComponent<CameraMove>().enabled = false;
					basket.SetActive(true);
					vics.SetActive(false);
				} 

			}
			if (Input.GetKeyDown(KeyCode.F11)) beginCraneGame = false;
		} else {
			if (craneParent.active){
				GetComponent<CameraMove>().startFocus = origStartFocus;
				ready = false;
				vics.SetActive(true);
				//if (GetComponent<CameraMove>().endFocus == craneCam){
				GetComponent<CameraMove>().enabled = true;
				GetComponent<CameraMove>().endFocus = origEndFocus;
				GetComponent<CameraMove>().endZoomAmt = origEndZoomAmt;
				GetComponent<CameraMove>().forceAmt = 0f;
				claw.transform.SetParent(craneParent.transform.GetChild(0), false);
				Camera.main.transform.SetParent(null, false);
				craneParent.SetActive(false);
			}
		}

		if (Input.GetKeyDown(KeyCode.F10) && !beginCraneGame) beginCraneGame = true;


	}
}
