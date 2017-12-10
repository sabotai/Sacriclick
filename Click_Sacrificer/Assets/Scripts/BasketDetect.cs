using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketDetect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col){
		if (col.tag == "organ"){
			int howMany = 0;
			switch (col.gameObject.GetComponent<OrganReset>().organType){
				case "heart":
					howMany = 200;
					break;
				case "lung":
					howMany = 100;
					break;
				case "stomach":
					howMany = 75;
					break;
				case "intestines":
					howMany = 50;
					break;
			}
			CraneGame.beginCraneGame = false;
			StartCoroutine(Camera.main.gameObject.GetComponent<CraneGame>().winCraneGame(howMany));
		}
	}
}
