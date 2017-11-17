using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Use this for initialization
	void Start () {
		GameObject sword = GameObject.Find("sword");
		sword.GetComponent<Cursword>().scaleSword = scaleSword;
		sword.GetComponent<Cursword>().maxSize = swordMax;
		sword.GetComponent<Cursword>().minSize = swordMin;
		sword.GetComponent<Cursword>().swordScaleDecay = swordScaleDecay;
		Camera.main.gameObject.GetComponent<BloodMeter>().useAutoJar = useAutoJar;
		Camera.main.gameObject.GetComponent<BloodMeter>().jarEfficiency = jarEfficiency;
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
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
}
