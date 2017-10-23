using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchFollow : MonoBehaviour {

	AudioSource audio;
	public float basePitch = 0.3f;
	float pitch;


	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
		pitch = audio.pitch;
	}
	
	// Update is called once per frame
	void Update () {
		float cps = Camera.main.gameObject.GetComponent<Sacrifice>().cps;

		pitch = basePitch + Mathf.Clamp(cps * 50f, 0f, 1f);    

		float smoothTime = 0.5F;
		float yVelocity = 0.0F;
		audio.pitch = Mathf.SmoothDamp(audio.pitch, pitch, ref yVelocity, smoothTime);
	}
}
