using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSwordHover : MonoBehaviour {

	[SerializeField] bool swordHovering = false;
	public float moodHoverDir = 1f;
	float labelOrigY;
	public float moodDirBounceAmplitude = 0.5f;
	public float bounceSpeed = 3f;
	float origMoodSpeedMult;
	public float hoverMoodSpeedMult = 2f;
	public AudioClip hoverClip;
	// Use this for initialization
	void Start () {
		labelOrigY = transform.GetChild(1).GetChild(0).localPosition.y;
		origMoodSpeedMult = gameObject.GetComponent<Mood>().moodSpeedMult;


		float manHoverMoodSpeedMult = GameObject.Find("DifficultyManager").GetComponent<DifficultyManager>().hoverMoodSpeedMult;
		if (hoverMoodSpeedMult != manHoverMoodSpeedMult) hoverMoodSpeedMult = manHoverMoodSpeedMult;
		float manMoodHoverDir = GameObject.Find("DifficultyManager").GetComponent<DifficultyManager>().moodHoverDir;
		if (moodHoverDir != manMoodHoverDir) moodHoverDir = manMoodHoverDir;
	}
	
	// Update is called once per frame
	void Update () {
		if (Drag.panMode){
			Vector3 camDir = Vector3.Normalize(Camera.main.transform.position - transform.position);
			Ray ray = new Ray(transform.position, camDir);
			RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit, 1000f,  LayerMask.GetMask("sword"))){
            	swordHovering = true;
            } else {
            	swordHovering = false;
            }

            if(swordHovering) {
            	gameObject.GetComponent<Mood>().moodDir = moodHoverDir;
				if (gameObject.GetComponent<Mood>().moodSpeedMult < origMoodSpeedMult * hoverMoodSpeedMult) {
					gameObject.GetComponent<Mood>().moodSpeedMult *= 1.1f;
					if (!GetComponent<AudioSource>().isPlaying && gameObject.GetComponent<Mood>().mood < 0.95f) GetComponent<AudioSource>().PlayOneShot(hoverClip, 1f - gameObject.GetComponent<Mood>().mood);
				}
				transform.GetChild(1).GetChild(0).localPosition += new Vector3(0f, Mathf.PingPong(Time.time * bounceSpeed, moodDirBounceAmplitude) - moodDirBounceAmplitude/2f, 0f); //bounce label
				//StartCoroutine(Pulsate.Pulse(gameObject.transform.GetChild(2).gameObject, 0.15f, 0.5f));
            	
            } else {
			//reset label position
			if (gameObject.GetComponent<Mood>().moodSpeedMult > origMoodSpeedMult) gameObject.GetComponent<Mood>().moodSpeedMult = origMoodSpeedMult;
			transform.GetChild(1).GetChild(0).localPosition = new Vector3(transform.GetChild(1).GetChild(0).localPosition.x, labelOrigY, transform.GetChild(1).GetChild(0).localPosition.z);
            }

		}
		
	}
}
