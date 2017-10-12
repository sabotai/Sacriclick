using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMovieTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// this line of code will make the Movie Texture begin playing
		((MovieTexture)GetComponent<RawImage>().mainTexture).loop = true;
		((MovieTexture)GetComponent<RawImage>().mainTexture).Play();
/*
		((MovieTexture)GetComponent<Renderer>().material.mainTexture).loop = true;
		((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
		*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
