using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mood : MonoBehaviour {


	public float mood = 0f;
	public float moodDir = 0f;
	public float moodShiftThresh = 0.5f;
	public float moodSpeedMult = 0.01f;
	public bool constrainMood = true;
	public float moodFailThresh = -.35f;


	void Awake(){

		//mood = transform.GetChild(1).GetComponent<ConsentMeter>().consentPct - 0.5f;
			mood = Random.Range(-0.5f, 1f);
		
	}
	// Use this for initialization
	void Start () {
		if (GetComponent<Pathfinder>().myCount < 15) mood = 1f;//go easy on them for the first few
			
		moodDir = neighborMood();
		//Debug.Log(this.transform.GetSiblingIndex() + ". " + mood + " w/dir of: " + moodDir);
	}
	
	// Update is called once per frame
	void Update () {
		if (moodShiftThresh < Mathf.Abs(moodDir)){
			mood += moodDir * moodSpeedMult * Time.deltaTime;
		}
		if (constrainMood) {
			mood = Mathf.Clamp(mood, -1f, 1f);
			moodDir = Mathf.Clamp(moodDir, -1f, 1f);
		}
		moodDir = neighborMood();
	}


	float neighborMood(){
		Transform victimParent;
		int sibIndex = this.transform.GetSiblingIndex();
		victimParent = transform.parent; //find my parent
		float neighborMoodAvg = 0f;
		int count = 0;
		if (sibIndex != victimParent.childCount - 1){
			neighborMoodAvg += victimParent.GetChild(sibIndex + 1).gameObject.GetComponent<Mood>().mood;
			count++;
		
			if (sibIndex != 0){
				neighborMoodAvg += victimParent.GetChild(sibIndex - 1).gameObject.GetComponent<Mood>().mood;
				count++;
			}
		}
		neighborMoodAvg /= count;
		return neighborMoodAvg;
	}
}
