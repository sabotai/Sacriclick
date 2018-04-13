using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheat : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.F12) && Input.GetKeyDown(KeyCode.F1))
			SceneManager.LoadScene(0);
		if (Input.GetKey(KeyCode.F12) && Input.GetKeyDown(KeyCode.F2))
			SceneManager.LoadScene(1);
		if (Input.GetKey(KeyCode.F12) && Input.GetKeyDown(KeyCode.G))
			Camera.main.GetComponent<Sacrifice>().easyMode = !Camera.main.GetComponent<Sacrifice>().easyMode;
	}
}
