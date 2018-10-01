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
	public float diff = 0.5f;
	public float diffProgression = 0.01f;
	public int initialFreebies = 15;
	public GameObject influenceSphere;
	public GameObject notifPrefab;
	public float moodLevel = 0f;

	void Awake(){

		//mood = transform.GetChild(1).GetComponent<ConsentMeter>().consentPct - 0.5f;
		//-0.45 - 1 = medium
		
	}
	// Use this for initialization
	void Start () {
		float manDiff = GameObject.Find("DifficultyManager").GetComponent<DifficultyManager>().diff;
		if (diff != manDiff) diff = manDiff; //override if differs
		float manDiffP = GameObject.Find("DifficultyManager").GetComponent<DifficultyManager>().diffProgression;
		if (diffProgression != manDiffP) diffProgression = manDiffP;


		float progress = transform.GetChild(1).GetComponent<UpdateLabel>().myCount * diffProgression;
		diff += progress;
		mood = Random.Range(-diff, 1f);

		int myCount = Camera.main.GetComponent<Sacrifice>().sacCount + transform.GetSiblingIndex() + 1;
		//Debug.Log("mycount= " + myCount);
			
		moodDir = neighborMood();
		if (myCount < initialFreebies) {
			mood = 0.99f;//go easy on them for the first few
			moodDir = 1f;
		}
		if (influenceSphere == null) influenceSphere = transform.GetChild(3).gameObject;
		//Debug.Log(this.transform.GetSiblingIndex() + ". " + mood + " w/dir of: " + moodDir);
		
	}
	
	// Update is called once per frame
	void Update () {
		if (mood < moodFailThresh) moodLevel = 5f; //worst level - !!!
		else if (mood < moodFailThresh / 2f) moodLevel = 2f; //second worst - !!
		else if (mood < moodFailThresh / 3f) moodLevel = 1f; //second worst - !!
		else moodLevel = 0f;

		if (moodShiftThresh < Mathf.Abs(moodDir)){
			mood += moodDir * moodSpeedMult * Time.deltaTime;
		}
		if (constrainMood) {
			mood = Mathf.Clamp(mood, -1f, 1f);
			moodDir = Mathf.Clamp(moodDir, -1f, 1f);
		}
		if (influenceSphere != null) {
			if (!influenceSphere.activeInHierarchy) moodDir = neighborMood();
		}
	}


	public float neighborMood(){
		Transform victimParent;
		int sibIndex = this.transform.GetSiblingIndex();
		float neighborMoodAvg = 0f;
		if (transform.parent != null && sibIndex != null){
			victimParent = transform.parent; //find my parent
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
		}
		return neighborMoodAvg;
	}

	void OnTriggerStay(Collider col){
		if (col.gameObject.name == "InfluenceSphere"){
			Mood infMood = col.transform.parent.gameObject.GetComponent<Mood>();
			if (infMood != null){
				moodDir = (moodDir + infMood.moodDir * 2f)/3f;
				moodSpeedMult = (moodSpeedMult + infMood.moodSpeedMult * 2f)/3f;
				mood = (mood * 99f + infMood.mood)/100f;
			}
		}
	}

	public void shiftMood(float amt){
		mood += amt;
		GameObject moodNotif = Instantiate(notifPrefab, transform);
		moodNotif.GetComponent<MoodNotification>().moodMod = amt;
		moodNotif.GetComponent<MoodNotification>().enabled = true;
	}

}
