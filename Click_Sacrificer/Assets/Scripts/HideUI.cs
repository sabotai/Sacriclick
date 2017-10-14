using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUI : MonoBehaviour {

	bool hide = false;
	public GameObject[] uiElements;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.H)) {
			hide = !hide;
			if (hide) {
				foreach (GameObject uie in uiElements){
					uie.SetActive(false);
				}
			} else {		
				foreach (GameObject uie in uiElements){

					uie.SetActive(true);
				}
			}
		}
	}
}
