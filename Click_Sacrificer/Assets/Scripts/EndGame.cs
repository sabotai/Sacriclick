using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour {

	public GameState gameState;
	public static int endState  = 0;
	public float defocusCap = 2f;
	float defocusAmt = 0f;
	public float defocusInc = 0.05f;
	public Text quote;
	public Sacrifice sac;
	public UIFadeOut instruct;
	public string[] quotes;
	// Use this for initialization
	void Start () {
		quote.text = quotes[(int)Random.Range(0, quotes.Length)];
	}
	
	void OnEnable(){
		endState = 1;
		defocusAmt = 0f;
	}

	// Update is called once per frame
	void Update () {
		if (endState == 1){
			//Debug.Log("end state begin");
			//defocus camera

			gameState.Defocus(defocusAmt);
			defocusAmt += defocusInc;

			GetComponent<Image>().color = new Color(0f, 0f, 0f, defocusAmt - 0.5f);

			if (defocusAmt > defocusCap) endState = 2;

			//fade in quote half way through sequence
			if (defocusAmt > defocusCap/2f)	quote.color = new Color(1f, 1f, 1f, quote.color.a + (Time.deltaTime/2));
			//endState = 2;

		} else if (endState == 2) {
			instruct.enabled = true;
			//instruct.color = new Color(instruct.color.r, instruct.color.g, instruct.color.b, instruct.color.a + (Time.deltaTime));
			if (Input.anyKey)	{
				PlayerPrefs.SetInt("init", 1);
				sac.easyMode = false;
				GameState.state = 1;
				endState = 0;
				//if (scoreCount > HighScore.minGodsAccess * 2) SceneManager.LoadScene(1);
				//else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}
}
