using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimHider : MonoBehaviour {

	public int numRevealed = 7;
	// Use this for initialization
	void Start () {
		
		Reveal();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Reveal(){
		if (!transform.GetChild(numRevealed - 1).gameObject.activeSelf) {
			for (int i = 0; i < numRevealed - 1; i++){
				transform.GetChild(i).gameObject.SetActive(true);
			}
		} 
		if (transform.GetChild(numRevealed).gameObject.activeSelf) {
			for (int i = numRevealed - 1; i < transform.childCount; i++){
				transform.GetChild(i).gameObject.SetActive(false);
			}
		} 
	}
}
