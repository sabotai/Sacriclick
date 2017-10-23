using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUI : MonoBehaviour {

	bool hide = false;
	public GameObject[] uiElements;
	GameObject uiCam;
	int origMask;
	// Use this for initialization
	void Start () {
		
		uiCam = GameObject.Find("3dUICamera");
		origMask = Camera.main.cullingMask;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.H)) {
			hide = !hide;
			if (hide) {
				foreach (GameObject uie in uiElements){
					uie.SetActive(false);
				}
				uiCam.SetActive(false);
				Camera.main.cullingMask = 0001111;
			} else {		
				foreach (GameObject uie in uiElements){

					uie.SetActive(true);
				}
				Camera.main.cullingMask = origMask;
				uiCam.SetActive(true);
			}
		}
	}
}
