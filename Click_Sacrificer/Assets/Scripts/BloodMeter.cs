using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BloodMeter : MonoBehaviour {

	public float bloodAmt = 100f;
	[SerializeField] float secondsRemaining = 0f;
	public float sacBloodValue = 10f;
	float origSacBloodValue;
	public float bloodSecondRatio = 0.1f;
	public bool failureAllowed = false;
	public Material bloodMat;
	public Color positiveBloodColor;
	public Color defaultBloodColor;
	public float bloodColorRecoverySpeed = 0.1f;
	public float restartTimeoutAmt = 3f;

	public float bloodScreenAmt = 13f;

	public bool failed;
	GameObject bloodCanvasItem;
	public VideoPlayer bloodPlayer;
	public AudioSource audsrc;
	public bool useMood = true;
	GameObject victims;
	public bool useAutoJar = true;
	public static bool firstClick = false;
	public bool slowBroker = true;
	public float slowBrokerPct = 0.5f;
	public RectTransform bloodUI;
	float bloodUIOrigY;
	public AudioSource rumbleAud;
	float lastShake;


	// Use this for initialization
	void Start () {
		bloodUIOrigY = bloodUI.localPosition.y;
		secondsRemaining = bloodAmt * bloodSecondRatio;
		failed = false;
		bloodCanvasItem = GameObject.Find("RawImage");
		victims = GameObject.Find("Victims");
		origSacBloodValue = sacBloodValue;
		firstClick = false;
		lastShake = 0f;

		if (ColorblindMode.cbMode){
			positiveBloodColor = ColorblindMode.cbGreen;
			defaultBloodColor = ColorblindMode.cbRed;
		}

	}
	
	// Update is called once per frame
	void Update () {


		if (GameState.state == -1 || Tips.displayingTip) bloodPlayer.Stop();
		if (!Tips.displayingTip && (GameState.state == 1 || GameState.state == 2|| GameState.state == 4)){
			if (bloodAmt < 0.01f && failureAllowed) failed = true; //start fail action frames
			if (failed && !GetComponent<Inventory>().failed && failureAllowed){
				GetComponent<Sacrifice>().Fail(restartTimeoutAmt, "THE GODS ARE DISPLEASED"); //make fail stuff happen
			} else {
				if (!firstClick && GetComponent<Sacrifice>().sacCount > 0){
					firstClick = true;
				}
				if (GetComponent<Sacrifice>().easyMode || GameState.state == -1){ //use easy mode as a debug to stop blood
					failureAllowed = false;

					bloodPlayer.Stop();
					//((MovieTexture)bloodCanvasItem.GetComponent<RawImage>().mainTexture).Stop();
				} else {
					//((MovieTexture)bloodCanvasItem.GetComponent<RawImage>().mainTexture).Play();
					bloodPlayer.Play();
					failureAllowed = true;

					if (firstClick) {
						float currentBSR = bloodSecondRatio;

						//slow it down if the temple is offscreen and/or if it is in broker mode
						if (Drag.panMode && slowBroker) currentBSR = bloodSecondRatio * (slowBrokerPct - (slowBrokerPct * (GetComponent<Drag>().pipMaterial.color.a / 2f)));
						bloodAmt -= Time.deltaTime * currentBSR; //1 ratio is 1:1 seconds to blood
						
					}

					secondsRemaining = bloodAmt / bloodSecondRatio;
				}

				if (bloodAmt > 0.01 && bloodAmt < (bloodScreenAmt * 0.09) && GetComponent<Inventory>().bloodJarNumber == 0 && !failed){ //signal to the player that their time is running out
					//pitchF.pitch *= 1.5f;
					rumbleAud.volume = 1f - (bloodAmt / bloodScreenAmt);
					//if (!rumbleAud.isPlaying) rumbleAud.Play();
					if (bloodAmt < (bloodScreenAmt * 0.07) && Time.time > lastShake + 0.05f) {

						//increase rumble amt as time runs out
						float rumbleAmt = 0.000000001f * (1f - (bloodAmt / bloodScreenAmt));

						StartCoroutine(Shake.ShakeThis(Camera.main.transform, 0.001f, rumbleAmt));
						lastShake = Time.time;
					}
				} else if (bloodAmt > bloodScreenAmt) { //increment blood jars if enough blood is shed
					GetComponent<Inventory>().createJar(false);

				} else if (bloodAmt < 0.01f && GetComponent<Inventory>().bloodJarNumber > 0 && useAutoJar){
					GetComponent<Inventory>().useJar(GetComponent<Inventory>().bloodSpawn.GetChild(GetComponent<Inventory>().bloodSpawn.childCount - 1).gameObject);
				} else {
					rumbleAud.volume -= 0.1f;

				}

				bloodAmt = Mathf.Clamp(bloodAmt, 0f, 20f); //dont allow to go below zero or over 30 for ui purposes
				Color bloodColor;
				bloodColor = bloodMat.GetColor("_TintColor");
				if (bloodColor.r != defaultBloodColor.r || bloodColor.g != defaultBloodColor.g || bloodColor.b != defaultBloodColor.b){
					bloodColor = Color.Lerp(bloodColor, defaultBloodColor, bloodColorRecoverySpeed);
				}
				
				float bloodPct = bloodAmt / 20f;
				float maxA = 0.22f;
				float bloodA = maxA - maxA * (bloodAmt / 20f);
				bloodMat.SetColor("_TintColor", new Color (bloodColor.r, bloodColor.g, bloodColor.b, bloodA));
				bloodUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bloodAmt); //sets the blood movement on screen


			}
		}
	}

	public void updateMood(){		
		if (useMood) {	
			//	old take from back method
			//	int leader = victims.transform.childCount - 1 - GetComponent<Sacrifice>().sacCount;
			int leader = 0 - GetComponent<Sacrifice>().sacCount;
			if (leader < 0) leader = 0;
			sacBloodValue = origSacBloodValue * victims.transform.GetChild(leader).gameObject.GetComponent<Mood>().mood * Mathf.Max(1f, victims.transform.GetChild(leader).gameObject.GetComponent<MultiSac>().multiplier / 3f);
			//Debug.Log("currentSacBloodValue = " + sacBloodValue);
		}
	}


}
