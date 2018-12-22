using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenHyperlink : MonoBehaviour {

	public string url;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OpenLink(){
		//allow game to quit after losing focus
		Application.runInBackground = true;
		
		Application.OpenURL(url);
	}
}
