using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFadeOut : MonoBehaviour {

	public float startTime = 0f;
	public float duration = 1f;
	public bool reverse = false;
	public GameObject disableMeAtFinish;
	public bool disableSelf = true;
	public GameObject victims, uuCam;

	// Use this for initialization
	void Start () {
		startTime = 0f;
		if (disableMeAtFinish == null) disableMeAtFinish = gameObject;
	}

	public void Begin(){

		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (startTime > 0f){

		    float t = (Time.time - startTime) / duration;
			if (reverse){
				GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(0f, 1f, t);
			} else {
				GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(1f, 0f, t);
			}
			if (GetComponent<CanvasGroup>().alpha <= 0f && !reverse){

				if (disableSelf) gameObject.SetActive(false);
			} 
			if (GetComponent<CanvasGroup>().alpha >= 1f && reverse){
				Camera.main.GetComponent<CameraMove>().enabled = true;
				uuCam.SetActive(true);
				victims.SetActive(true);
				disableMeAtFinish.SetActive(false);
				reverse = !reverse;
				Begin();

			} 

		}
	}
}
