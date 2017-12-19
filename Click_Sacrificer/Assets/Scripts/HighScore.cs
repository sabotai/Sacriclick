using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour {

	public Text highScoreEasyUI;
	public Text highScoreMediumUI;
	public Text highScoreHardUI;
	bool allowHighScore = true;
	// Use this for initialization
	void Start () {
		highScoreEasyUI.text = PlayerPrefs.GetInt("highScoreEasy") + "";
		highScoreMediumUI.text = PlayerPrefs.GetInt("highScoreMedium") + "";
		highScoreHardUI.text = PlayerPrefs.GetInt("highScoreHard") + "";
		allowHighScore = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!GetComponent<Sacrifice>().easyMode && allowHighScore){
			switch(DifficultyManager.currentDifficulty){
				case 1:
				if (GetComponent<Sacrifice>().sacCount > PlayerPrefs.GetInt("highScoreEasy")){
					PlayerPrefs.SetInt("highScoreEasy", GetComponent<Sacrifice>().sacCount);
					highScoreEasyUI.text = PlayerPrefs.GetInt("highScoreEasy") + "";
				}
					break;
				case 2:
					if (GetComponent<Sacrifice>().sacCount > PlayerPrefs.GetInt("highScoreMedium")){
						PlayerPrefs.SetInt("highScoreMedium", GetComponent<Sacrifice>().sacCount);
						highScoreMediumUI.text = PlayerPrefs.GetInt("highScoreMedium") + "";
					}
					break;
				case 3:
					if (GetComponent<Sacrifice>().sacCount > PlayerPrefs.GetInt("highScoreHard")){
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
