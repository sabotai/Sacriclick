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
			CraneGame.beginCraneGame = false;
			StartCoroutine(Camera.main.gameObject.GetComponent<CraneGame>().winCraneGame());
		}
	}
}
