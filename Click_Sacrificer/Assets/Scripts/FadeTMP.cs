using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeTMP : MonoBehaviour {

	TextMeshProUGUI myText;
	public Color32 startColor, endColor;
	public float duration = 1f;
	public bool fadeIn, fadeOut;
	float startTime = 0f;


	// Use this for initialization
	void Start () {
		myText = GetComponent<TextMeshProUGUI> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (fadeOut){

		    float t = (Time.time - startTime) / duration;
			myText.color = Color32.Lerp(startColor, endColor, Mathf.SmoothStep(0f, 1f, t));
			if (myText.color == endColor) {
				//fadeOut = false;
				FadeIn();
			}
		} else if (fadeIn){
		    float t = (Time.time - startTime) / duration;
	        myText.color = Color32.Lerp(endColor, startColor, Mathf.SmoothStep(0f, 1f, t));
			if (myText.color == startColor) fadeIn = false;
		}


	}
	public void FadeIn(){
		fadeIn = true;
		fadeOut = false;
		startTime = Time.time;

		//myText.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));


       // TextMeshPro textmeshPro = GetComponent<TextMeshPro>();

        
	}

	public void FadeOut(){
		fadeOut = true;
		fadeIn = false;
		startTime = Time.time;
	}
}