using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighScore : MonoBehaviour {

	public Text highScoreEasyUI;
	public Text highScoreMediumUI;
	public Text highScoreHardUI;
	public bool allowHighScore = true;
	public GameObject godsModeOption;
	public GameObject altBeginButton;
	public Slider difficultySlider;
	public static int minGodsAccess = 500;
	public static int minEasyAccess = 100;
	public static int minSceneAccess = 200;
	// Use this for initialization
	void Start () {
		highScoreEasyUI.text = PlayerPrefs.GetInt("highScoreEasy") + "";
		highScoreMediumUI.text = PlayerPrefs.GetInt("highScoreMedium") + "";
		highScoreHardUI.text = PlayerPrefs.GetInt("highScoreHard") + "";
		allowHighScore = true;

		if (PlayerPrefs.GetInt("highScoreEasy") > minGodsAccess * 3 || PlayerPrefs.GetInt("highScoreMedium") > minGodsAccess * 2 || PlayerPrefs.GetInt("highScoreHard") > minGodsAccess){
			godsModeOption.SetActive(true);

		}
		if (PlayerPrefs.GetInt("highScoreEasy") > minEasyAccess * 3 || PlayerPrefs.GetInt("highScoreMedium") > minEasyAccess * 2 || PlayerPrefs.GetInt("highScoreHard") > minEasyAccess){
			difficultySlider.minValue = 1;
		}
		if (PlayerPrefs.GetInt("highScoreEasy") > minSceneAccess * 3 || PlayerPrefs.GetInt("highScoreMedium") > minSceneAccess * 2 || PlayerPrefs.GetInt("highScoreHard") > minSceneAccess){
			altBeginButton.SetActive(true);
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


	public void loadScene(){
		if (SceneManager.GetActiveScene().buildIndex == 0){
			SceneManager.LoadScene(1);
		}
		else {
			SceneManager.LoadScene(0);
		}
	}
}
