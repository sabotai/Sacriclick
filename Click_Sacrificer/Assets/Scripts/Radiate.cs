﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radiate : MonoBehaviour {

	public bool pulse = false;
	public bool rad = false;
	public bool flick = false;
	public float speed = 1f;
	public float maxLight = 1f;
	public Color brightColor;
	public Color darkColor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float sAmt = Mathf.PingPong(Time.time * 1.2f, maxLight);

		if (!Input.GetButton("Sacrifice")) {
			if (pulse){
				pulseColor(gameObject, darkColor, brightColor, sAmt);
			} 
			if (rad){
				radiate(gameObject);
			} 

			if (flick){
				flicker(gameObject, maxLight, sAmt);
			}
		}
	}

	void pulseColor(GameObject pulseObj, Color c1, Color c2, float amt){
		Color pulseColor = pulseObj.GetComponent<MeshRenderer> ().material.color;
		pulseColor = Color.Lerp(c1, c2, amt);
		pulseObj.GetComponent<MeshRenderer> ().material.color = pulseColor;

	}

	public static IEnumerator onePulse(GameObject pulseObj, Color c1, Color c2){


		Color pulseColor = pulseObj.GetComponent<MeshRenderer> ().material.color;

		float duration = 5f;
		float elapsed = 0f;
		float sspeed = 0.01f;
		while( elapsed < duration ){
			float pct = elapsed / duration;
			float amt = 2f * pct; 
			if (pct >= 0.5f){ //0.5 needs to be 1, 1, needs to be 0
				amt = 2 - (pct * 2f);
				//Debug.Log("down pulsing?");
			} 
			//Debug.Log("onePulse amt = " + amt);
			pulseColor = Color.Lerp(c1, c2, amt);

			pulseObj.GetComponent<MeshRenderer> ().material.color = pulseColor;
			elapsed += sspeed;

			yield return null;
		}
		Debug.Log("done with onePulse");
		

		
	}
	public static IEnumerator onePulse(GameObject pulseObj, Color c1, Color c2, float duration, float sppeed){


		Color pulseColor = pulseObj.GetComponent<MeshRenderer> ().material.color;

		float elapsed = 0f;
		
		while( elapsed < duration ){
			float pct = elapsed / duration;
			float amt = 2f * pct; 
			if (pct >= 0.5f){ //0.5 needs to be 1, 1, needs to be 0
				amt = 2 - (pct * 2f);
				//Debug.Log("down pulsing?");
			} 
			//Debug.Log("onePulse amt = " + amt);
			pulseColor = Color.Lerp(c1, c2, amt);

			pulseObj.GetComponent<MeshRenderer> ().material.color = pulseColor;
			elapsed += sppeed;

			yield return null;
		}
		Debug.Log("done with onePulse");
		

		
	}
	public static IEnumerator oneSmoothPulse(GameObject pulseObj, Color c1, Color c2, float speeed){


		Color pulseColor = pulseObj.GetComponent<MeshRenderer> ().material.color;


   		float smoothTime = speeed;
    	float yVelocity = 0.0F;
    	bool goingDown = false;
    	float target = 1f;
		
		float sAmt = 0.01f;
		if (pulseColor == c1){ //prevent from holding the c1 from restarting with each coroutine start
			while(sAmt >= 0.01f){
				
				sAmt = Mathf.SmoothDamp(sAmt, target, ref yVelocity, smoothTime);
				if (sAmt > 0.99f && !goingDown){ //reverse dir
					target = 0f;
					goingDown = true;
					//Color colorHold = c1;
					//c1 = c2;
					//c2 = colorHold;
				}
				//Debug.Log("oneSmoothPulse amt = " + sAmt);
				pulseColor = Color.Lerp(c1, c2, sAmt);

				pulseObj.GetComponent<Light>().intensity = 1f - sAmt;
				pulseObj.GetComponent<MeshRenderer>().material.color = pulseColor;
				yield return null;
			}
			pulseObj.GetComponent<MeshRenderer>().material.color = c1;
		}
		

		
	}

	void radiate(GameObject radObj){
		Color radColor = radObj.GetComponent<MeshRenderer> ().material.color;
		float rSpeed = 0.3f;
		float gSpeed = 0.8f;
		float bSpeed = 0.2f;
		radColor.r = (Mathf.Sin(rSpeed * Time.time) + 1f) / 2f;
		radColor.g = ((Mathf.Sin(gSpeed * Time.time) + 1f) / 2f);
		radColor.b = ((Mathf.Sin(bSpeed * Time.time) + 1f) / 2f);
		//Debug.Log("red = " + radColor.r + "  g/b = " + radColor.b);
		radObj.GetComponent<MeshRenderer> ().material.color = radColor;
	}

	void flicker(GameObject flickObj, float max, float amt){

   		//float smoothTime = speed;
    	//float yVelocity = 0.0F;
    	//float target = max;
		if (flickObj.GetComponent<Light>() != null){
			flickObj.GetComponent<Light>().intensity = amt;
		}
	}
	
}
