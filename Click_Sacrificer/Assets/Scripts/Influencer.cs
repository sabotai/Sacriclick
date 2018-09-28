using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influencer : MonoBehaviour {

	bool influencer = false;
	public float decayRate;
	public float initInfluenceLevel = 92f;
	public AudioClip influenceClip;
	
	// Use this for initialization
	void Start () {
		
	}
	void OnEnable(){
		influencer = true;
		transform.localScale = new Vector3(initInfluenceLevel, initInfluenceLevel, initInfluenceLevel);
	}
	// Update is called once per frame
	void Update () {
		if (influencer){
			//if falls below min influence
			if (transform.localScale.x < 1f) {
				influencer = false;
				gameObject.SetActive(false);
			} else {
				if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
				GetComponent<AudioSource>().volume = transform.localScale.x / initInfluenceLevel;
				transform.localScale *= (1f - (decayRate * Time.deltaTime));

				//if sacrificed, remove influence
				if (!transform.parent.gameObject.GetComponent<Mood>()) decayRate *= 100f;
			}
		} 

	}
}
