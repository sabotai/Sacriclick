using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighScore : MonoBehaviour {

	public Text highScoreEasyUI;
	public Text highScoreMediumUI;
	public Text highScoreHardUI;
	bool allowHighScore = true;
	public GameObject godsModeOption;
	public static int minGodsAccess = 100;
	// Use this for initialization
	void Start () {
		highScoreEasyUI.text = PlayerPrefs.GetInt("highScoreEasy") + "";
		highScoreMediumUI.text = PlayerPrefs.GetInt("highScoreMedium") + "";
		highScoreHardUI.text = PlayerPrefs.GetInt("highScoreHard") + "";
		allowHighScore = true;

		if (PlayerPrefs.GetInt("highScoreEasy") > minGodsAccess * 3 || PlayerPrefs.GetInt("highScoreMedium") > minGodsAccess * 2 || PlayerPrefs.GetInt("highScoreHard") > minGodsAccess){
			godsModeOption.SetActive(true);


		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!GetComponent<Sacrifice>().easyMode && allowHighScore){
			switch(DifficultyManager.currentDifficulty){
				case 1:
				if (GetComponent<Sacrifice>().scoreCount > PlayerPrefs.GetInt("highScoreEasy")){
					PlayerPrefs.SetInt("highScoreEasy", GetComponent<Sacrifice>().sacCount);
					highScoreEasyUI.text = PlayerPrefs.GetInt("highScoreEasy") + "";
				}
					break;
				case 2:
				if (GetComponent<Sacrifice>().scoreCount > PlayerPrefs.GetInt("highScoreMedium")){
						PlayerPrefs.SetInt("highScoreMedium", GetComponent<Sacrifice>().sacCount);
						highScoreMediumUI.text = PlayerPrefs.GetInt("highScoreMedium") + "";
					}
					break;
				case 3:
				if (GetComponent<Sacrifice>().scoreCount > PlayerPrefs.GetInt("highScoreHard")){
						PlayerPrefs.SetInt("highScoreHard", GetComponent<Sacrifice>().sacCount);
						highScoreHardUI.text = PlayerPrefs.GetInt("highScoreHard") + "";
					}
					break;

			}
		} else {
			allowHighScore = false;
		}
	}
}
