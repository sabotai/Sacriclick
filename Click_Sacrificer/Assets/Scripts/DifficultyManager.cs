using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager : MonoBehaviour {

	public bool scaleSword = true;//cursword
	public Vector3 swordMax = new Vector3(4.8f, 4.8f, 4.8f);//cursword
	public Vector3 swordMin = new Vector3(0.05f, 0.05f, 0.05f);//cursword
	public float swordScaleDecay = 2f;
	public float waySpeed = 5f;//pathfinder
	public bool useAutoJar = true;
	public float jarEfficiency = 0.3f;//blood meter
	public float sacBloodValue = 1f;//blood meter
	public float bloodSecondRatio = 1f; //bloodmeter
	//public int howManySpawn = 35; //victimgentoo
	public float moodFailThresh = -0.35f;
	public float diff = 0.2f;
	public float diffProgression = 0.005f;
	public float moodSpeedMult = 0.1f;
	public bool constrainMood = true;
	public float hoverMoodSpeedMult = 2f;
	public float moodHoverDir = 1f;
	public GameObject victimPrefab;
	public int initFreebies = 15;
	public float autosacDuration = 5f;
	public Slider difficultySlider;
	public static int currentDifficulty;
	public float easyDiff = 3f;
	public float mediumDiff = 8f;
	public float hardDiff = 11f;

	void Awake(){

		victimPrefab.GetComponent<Mood>().initialFreebies = initFreebies;
	}

	// Use this for initialization
	void Start () {
		GetComponent<Autosac>().duration = autosacDuration;
		GameObject sword = GameObject.Find("sword");
		sword.GetComponent<Cursword>().scaleSword = scaleSword;
		sword.GetComponent<Cursword>().maxSize = swordMax;
		sword.GetComponent<Cursword>().minSize = swordMin;
		sword.GetComponent<Cursword>().swordScaleDecay = swordScaleDecay;
		Camera.main.gameObject.GetComponent<BloodMeter>().useAutoJar = useAutoJar;
		Camera.main.gameObject.GetComponent<Inventory>().jarEfficiency = jarEfficiency;
		Camera.main.gameObject.GetComponent<BloodMeter>().sacBloodValue = sacBloodValue;
		Camera.main.gameObject.GetComponent<BloodMeter>().bloodSecondRatio = bloodSecondRatio;
		Camera.main.gameObject.GetComponent<BloodMeter>().useAutoJar = useAutoJar;

		GameObject victimParent = GameObject.Find("Victims");
		GameObject[] victimz = new GameObject[victimParent.transform.childCount]; //setup victimz array with space for each child
		for (int i = 0; i < victimz.Length; i++){ //assign each one
			victimz[i] = victimParent.transform.GetChild(i).gameObject;
		}
		foreach (GameObject vic in victimz){
			if (vic.GetComponent<Pathfinder>() != null){
				vic.GetComponent<Pathfinder>().waySpeed = waySpeed;
			} else if (Camera.main.gameObject.GetComponent<MasterWaypointer>() != null){
				Camera.main.gameObject.GetComponent<MasterWaypointer>().waySpeed = waySpeed;
			}
			vic.GetComponent<CheckSwordHover>().hoverMoodSpeedMult = hoverMoodSpeedMult;
			vic.GetComponent<CheckSwordHover>().moodHoverDir = moodHoverDir;
			vic.GetComponent<Mood>().moodFailThresh = moodFailThresh;
			vic.GetComponent<Mood>().diff = diff;
			vic.GetComponent<Mood>().diffProgression = diffProgression;
			vic.GetComponent<Mood>().constrainMood = constrainMood;
		}

		currentDifficulty = PlayerPrefs.GetInt("difficulty");

		SetDifficulty((float)PlayerPrefs.GetInt("difficulty"));
	}

	public void SetDifficultyDelay(float newDiff){

		PlayerPrefs.SetInt("difficulty", (int)newDiff);
		if (!BloodMeter.firstClick) SetDifficulty(newDiff);
	}

	public void SetDifficulty(float newDiff){
		float diffNumber = mediumDiff;
		if (newDiff == 1f) diffNumber = easyDiff;
		else if (newDiff == 2f) diffNumber = mediumDiff;
		else if (newDiff == 3f) diffNumber = hardDiff;

		diffProgression = (float)(diffNumber * 0.0005f);
		Camera.main.gameObject.GetComponent<BloodMeter>().bloodSecondRatio = 0.75f + (0.75f * ((diffNumber-1)/9f));

		GameObject victimParent = GameObject.Find("Victims");
		GameObject[] victimz = new GameObject[victimParent.transform.childCount]; //setup victimz array with space for each child
		for (int i = 0; i < victimz.Length; i++){ //assign each one
			victimz[i] = victimParent.transform.GetChild(i).gameObject;
		}
		foreach (GameObject vic in victimz){
			if (vic.GetComponent<Pathfinder>() != null){
				vic.GetComponent<Pathfinder>().waySpeed = waySpeed;
			} else if (Camera.main.gameObject.GetComponent<MasterWaypointer>() != null){
				Camera.main.gameObject.GetComponent<MasterWaypointer>().waySpeed = waySpeed;
			}
			vic.GetComponent<CheckSwordHover>().hoverMoodSpeedMult = hoverMoodSpeedMult;
			vic.GetComponent<CheckSwordHover>().moodHoverDir = moodHoverDir;
			vic.GetComponent<Mood>().moodFailThresh = moodFailThresh;
			vic.GetComponent<Mood>().diff = diff;
			vic.GetComponent<Mood>().diffProgression = diffProgression;
			vic.GetComponent<Mood>().constrainMood = constrainMood;
		}

		PlayerPrefs.SetInt("difficulty", (int)newDiff);
		difficultySlider.value = newDiff;
	}
}
