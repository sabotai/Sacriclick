using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUI : MonoBehaviour {

	public static bool hide = false;
	public GameObject[] uiElements;
	GameObject[] labels;
	GameObject uiCam;
	int origMask;
	GameObject sacrificer;

	// Use this for initialization
	void Start () {
		labels = GameObject.FindGameObjectsWithTag("label");
		
		uiCam = GameObject.Find("3dUICamera");
		origMask = Camera.main.cullingMask;
		sacrificer = Camera.main.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameState.state == 1 && hide) {
			hide = false;
			Unhide();
		}
		if (GameState.state == 0 && !hide) {
			hide = true;
			Hide();
		}

		if (Input.GetKeyDown(KeyCode.H) && !CraneGame.beginCraneGame) {
			hide = !hide;
			if (hide) {
				Hide();
			} else {	
				Unhide();	
			}
		}
	}

	void Hide(){
		labels = GameObject.FindGameObjectsWithTag("label");
		foreach (GameObject uie in uiElements){
			uie.SetActive(false);
		}

		foreach (GameObject label in labels){
			label.SetActive(false);
		}

		uiCam.SetActive(false);
		Camera.main.cullingMask = 0001111111;

	}
	void Unhide(){

				foreach (GameObject uie in uiElements){

					uie.SetActive(true);
				}

				foreach (GameObject label in labels){
					label.SetActive(true);
				}
				Camera.main.cullingMask = origMask;
				uiCam.SetActive(true);
	}
}
