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
	public Dropdown cbUI;
	public PostProcessVolume[] options;
	public static int colorOption;

	void Start(){
		//turn everything off
		for (int i = 0; i < options.Length; i++){
			options[i].weight = 0f;
		}

		cbGreen = new Color(0f, 0.543f, 1f, 1f);
		cbRed = new Color(1f, 0.64f, 0f, 1f);
		int cbInt = PlayerPrefs.GetInt("cbMode");
		if (cbInt == 1) {
			cbMode = true;
		} else if (cbInt == -1) {
			cbMode = false;
		}
		Debug.Log("cbMode set to: " + cbMode);
		cbUI.value = PlayerPrefs.GetInt("color");
		SetColor(PlayerPrefs.GetInt("color"));
	}
	// Use this for initialization
	public void SetColor (int setting) {
		PlayerPrefs.SetInt("color", setting);
		colorOption = setting;
		cbMode = false;
		PlayerPrefs.SetInt("cbMode", -1);
		Camera.main.GetComponent<ContrastStretch>().enabled = false;

		for (int i = 0; i < options.Length; i++){

			options[i].weight = 0f;
		}
		if (setting < options.Length) options[setting].weight = 1f;

		if (colorOption == 0){
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = true;

		} else if (colorOption == 1){
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = true;

		} else if (colorOption == 2){
			Camera.main.GetComponent<ContrastStretch>().enabled = true;
		}  else if (colorOption == 3){
			Camera.main.GetComponent<ContrastStretch>().enabled = true;
			//options[options.Length - 1].weight = 1f;
			options[0].weight = 1f;
		}  else if (colorOption == 4){
			Camera.main.GetComponent<ContrastStretch>().enabled = true;
			options[options.Length - 1].weight = 1f;
			options[1].weight = 1f;
		} else if (colorOption > 4){
			cbMode = true;
			PlayerPrefs.SetInt("cbMode", 1);
			options[3].weight = 1f;

			//Camera.main.GetComponent<ContrastStretch>().enabled = !cbMode;
			Camera.main.GetComponent<ColorCorrectionCurves>().enabled = false;

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
