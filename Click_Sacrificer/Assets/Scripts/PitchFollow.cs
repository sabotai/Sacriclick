using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchFollow : MonoBehaviour {

	AudioSource audio;
	public float basePitch = 0.3f;
	public float maxPitch = 1.5f;
	public float followSpeed = 0.5f;
	public float pitch;
	public AudioClip[] clips;
	float originalVol;
	public float adjustedVol = 0.4f;


	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
		pitch = audio.pitch;

		audio.clip = clips[(int)Random.Range(0, clips.Length)];
		audio.Stop();
		audio.Play();
		originalVol = audio.volume;
	}
	
	// Update is called once per frame
	void Update () {
		float cps = Camera.main.gameObject.GetComponent<Sacrifice>().cps;

		pitch = basePitch + Mathf.Clamp(cps * 50f, 0f, 1f); 
		pitch = Mathf.Clamp(pitch, basePitch, maxPitch);

		float smoothTime = followSpeed;
		float yVelocity = 0.0F;
		if (GameState.state == 1 || GameState.state == 2) audio.pitch = Mathf.SmoothDamp(audio.pitch, pitch, ref yVelocity, smoothTime);
		if (Tips.displayingTip) audio.volume = adjustedVol; else audio.volume = originalVol;
	}
}
