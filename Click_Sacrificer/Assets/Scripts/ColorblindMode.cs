using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorblindMode : MonoBehaviour {

	public static bool cbMode;
	public static Color cbRed = new Color(1f, 0.64f, 0f, 1f);
	public static Color cbGreen = new Color(0f, 0.543f, 1f, 1f);
	public Toggle cbToggle;

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
		
	}
	// Use this for initialization
	public void SetCB (bool setting) {
		cbMode = setting;
		int cbInt = -1;
		if (cbMode) cbInt = 1;
		PlayerPrefs.SetInt("cbMode", cbInt);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
