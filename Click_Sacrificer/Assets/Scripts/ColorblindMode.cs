using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Rendering.PostProcessing;

public class ColorblindMode : MonoBehaviour {

	public static bool cbMode;
	public static Color cbRed = new Color(1f, 0.64f, 0f, 1f);
	public static Color cbGreen = new Color(0f, 0.543f, 1f, 1f);
	public Toggle cbToggle;
	public PostProcessVolume[] options;
	public static int colorOption;

	void Start(){
		cbGreen = new Color(0f, 0.543f, 1f, 1f);
		cbRed = new Color(1f, 0.64f, 0f, 1f);
		int cbInt = PlayerPrefs.GetInt("cbMode");
		if (cbInt == 1) {
			cbMode = true;
		} else if (cbInt == -1) {
			cbMode = false;
		}
		Debug.Log("cbMode set to: " + cbMode);
		cbToggle.isOn = cbMode;
		SetColor(colorOption);
	}
	// Use this for initialization
	public void SetColor (int setting) {
		colorOption = setting;
		cbMode = false;
		PlayerPrefs.SetInt("cbMode", -1);
		Camera.main.GetComponent<ContrastStretch>().enabled = false;

		if (colorOption == 0){
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = true;

		} else if (colorOption == 1){
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = true;

		} else if (colorOption == 2){
			Camera.main.GetComponent<ContrastStretch>().enabled = true;
		} else if (colorOption == options.Length - 1){
			cbMode = true;
			PlayerPrefs.SetInt("cbMode", 1);

			//Camera.main.GetComponent<ContrastStretch>().enabled = !cbMode;
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = false;

			/*
			if (cbMode){
				options[setting].weight = 0f;
				cbVol.weight = 1f;
			} else {
				defaultVol.weight = 1f;
				cbVol.weight = 0f;
			}
			*/
		}
		for (int i = 0; i < options.Length; i++){

			options[i].weight = 0f;
		}
		options[setting].weight = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
