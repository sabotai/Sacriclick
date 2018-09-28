using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class Sacrifice : MonoBehaviour {

	public GameObject headPrefab;
	public GameObject clickable;
	public GameObject sword;
	Vector3 swordOrigScale;
	private Vector3 clickOrigScale;
	//public AudioClip[] screams;
	public AudioClip[] rumbleSound;
	public bool advance = false;
	public Text sacCountDisplay;
	public Text cpsDisplay;
	public AudioSource audio;
	public AudioSource audio2;
	private GameObject sun;
	public float cpmDuration = 5;
	public float startTime;
	public float cpmMag = 0.01f;
	public float cpm;
	public float cps;
	public float cpf;
	public float maxCps = 15f;
	float[] cpsSamples;
	float[] cpmSamples;
	float pCps;
	float ppCps;
	public bool easyMode = false;
	public bool limitAvailSac = true;
	public bool sacReady = true;
	public int sacCount = 0;
	public int scoreCount = 0;
	public int expenses = 0;
	public bool failed = false;
	float failedTime = 0.0f;
	GameObject failObj;
	public static bool playScreams = false;
	public Vector3 bloodOffset;
	GameObject selectedObj;
	GameObject diffManager;
	public float autoThresh = 10;
	bool hovering = false;
	public GameObject lightParent;
	public AudioClip templeHoverClip;
	EdgeDetection edge;
	public float pitchMin, pitchMax;
	public AudioClip goodSacClip, badSacClip, loseClip;
	float fontSize;
	public float nLerpTime = 0.0015f;
	public GameObject bloodEffect, fireEffect;
	public Transform sacrificeSpot;
	public Transform[] failDisable;

	void Awake(){
		sacCount = 0;
		scoreCount = 0;
		expenses = 0;
	}
	// Use this for initialization
	void Start () {
		edge = GetComponent<EdgeDetection>();
		easyMode = false;
		clickable = GameObject.Find("click-toy");
		clickOrigScale = clickable.transform.localScale;
		swordOrigScale = sword.transform.localScale;
		sun = GameObject.Find("Sun");
		diffManager = GameObject.Find("DifficultyManager");
		//audio2 = GetComponent<AudioSource>();
		//audio = clickable.GetComponent<AudioSource>();
		cpm = 0f;
		cps = 0f;
		cpf = 0f;
		sacCount = 0;
		startTime = Time.time;

		sacCountDisplay.text = sacCount + "";
		cpsDisplay.text = cpm/60 + " /second";
		failObj = GameObject.Find("GameOver");
		failObj.GetComponent<Text>().text = "";

		int cpsSampleNum = 60; //based on 60fps
		cpsSamples = new float[cpsSampleNum];
		for(int i = 0; i < cpsSamples.Length; i++){
			cpsSamples[i] = 0f;
		}
		int howMany = 3600;//based on 60 fps * 60 seconds
 		cpmSamples = new float[howMany];
		for(int i = 0; i < cpmSamples.Length; i++){
			cpmSamples[i] = 0f;
		}
		fontSize = (float)failObj.GetComponent<Text>().fontSize;

	}
	public void SetEasyMode(bool setE){
		easyMode = setE;
	}

	public void SetScreams(bool setS){
		playScreams = setS;
	}

	// Update is called once per frame
	void Update () {
		if (scoreCount != sacCount - expenses){
			scoreCount = sacCount - expenses;
			sacCountDisplay.text = " " + scoreCount;//Sacrifices";
		}
		if (!CraneGame.beginCraneGame && !Tips.displayingTip){
			//if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(0);
			if (Drag.panMode){
				clickable.GetComponent<MeshRenderer>().material.color = Color.black;
			} 
			//cps = cpm/60f;
			
			advance = false;
			//we use rays to project invisible lines at a distance
			//in this case, we are using the ScreenPointToRay function of our main camera
			//to convert the mouse position on the screen into a direction projected through the screen 
			//(in class, i drew a dinosaur on the other side)
			Ray beam = Camera.main.ScreenPointToRay(Input.mousePosition);

			//draw our debug ray so we can see inside unity
			//the ray starts from beam.origin and is drawn to 1000 units in the rays direction (beam.direction)
			Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.red);

			//declare and initialize our raycasthit to store hit information
			RaycastHit beamHit = new RaycastHit();

			if (GameState.state == 1){
				if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("click-toy"))){
					if (!hovering){ //use hovering to efficiently only run once
						lights(0.15f);
						lightParent.GetComponent<AudioSource>().PlayOneShot(templeHoverClip);
					}
					hovering = true;
					if (!failed) edge.edgeExp = 0.3f;
					//old shrink cursword code when hovering over clickable 
					/*
					if (beamHit.collider.gameObject == clickable) {
						sword.GetComponent<Cursword>().hideCursor = true;
						sword.GetComponent<Cursword>().currentSize *= 0.7f;
					} else {
						sword.GetComponent<Cursword>().hideCursor = false;
					}
					*/

					//if the raycast hits a rigidbody and the player is pressing the right mouse button
					bool clicking = false;
					if (easyMode){
						clicking = Input.GetButton("Sacrifice");
					} else {
						clicking = Input.GetButtonDown("Sacrifice");
					}

					if (clicking){
						//no longer displaying sword in blood mode
						//Vector3 swordOrigPos = sword.transform.localPosition;
						//StartCoroutine(Pulsate.PulsePos(sword.transform.gameObject, 0.15f, swordOrigScale.x / 20f, swordOrigPos));

						if (beamHit.collider.gameObject == clickable || beamHit.collider.gameObject.tag == "click-toy"){
							if (!limitAvailSac || sacReady){ //if limited, check if ready
								if (GameState.state == 1){
									sacReady = false;

									if (diffManager.GetComponent<MasterWaypointer>() != null){
										//Debug.Log("found master waypointer");
										if (diffManager.GetComponent<MasterWaypointer>().vicReady){
											
												DoSacrifice(clickable);

										}


									} else {
											DoSacrifice(clickable);
									}
									//Debug.Log("sac reset");
								}
								
							}
						}
					}

				} else {
					hovering = false;
					edge.edgeExp = 0.2f;
					lights(0.05f);
				}
			} else { //in pan mode
				hovering = false;
				lights(0.15f);
			}
			calcCPS();
		
		}
	}

	void lights(float intens){
			for (int i = 0; i < lightParent.transform.childCount; i++){
				lightParent.transform.GetChild(i).gameObject.GetComponent<Light>().intensity = intens;
				//lightParent.transform.GetChild(i).gameObject.GetComponent<Flicker>().Awake();
			}

	}

	void calcCPS(){
		int fps = (int)(1f / Time.deltaTime);
		int useFrameAmt = (int)Mathf.Clamp(fps, 0f, 60f);

		//move everything down by one
		for (int i = useFrameAmt - 1; i > 0; i--){
			cpsSamples[i] = cpsSamples[i - 1];
		}
		cpsSamples[0] = cpf;


		float totalCps = 0f;
		for (int i = 0; i < useFrameAmt; i++){
			totalCps += cpsSamples[i];
		}
		cps = totalCps;
		cpf = 0f;

			calcCPM();

			if (sun.GetComponent<SunPct>() != null && sun.GetComponent<SunPct>().manControl == false )
				sun.GetComponent<SunPct>().rotAmt = (sun.GetComponent<SunPct>().rotAmt * 0.7f) + ((cps / maxCps) * 0.3f);

		cpsDisplay.text = (int)cps + "/s.  " + (int)cpm + "/m.";


		//if fast enough to generate autosac
		if (cps > autoThresh){
			if (GetComponent<Inventory>().autosacNumber < 8){
				GetComponent<Inventory>().createAuto();
				
				//reset the samples so they cant spam it
				for (int i = 0; i < cpsSamples.Length; i++){
					cpsSamples[i] = 0f;
				}
			}
		}
		//cpm = 0;
	}
	void calcCPM(){

		int fpm = (int)(1f / Time.deltaTime) * 60;
		int useFrameAmtMin = (int)Mathf.Clamp(fpm, 0f, 3600f);
		//move everything down by one
		for (int i = useFrameAmtMin - 1; i > 0; i--){

			cpmSamples[i] = cpmSamples[i - 1];

			//Debug.Log(cpsSamples[i]);
		}
		cpmSamples[0] = cps * Time.deltaTime;

		float sumCpm = 0f;
		for (int i = 0; i < useFrameAmtMin; i++){
			sumCpm += cpmSamples[i];
		}
		//sumCpm /= cpmSamples.Length;
		//Debug.Log("sumCPM = " + sumCpm);

		cpm = sumCpm; //update to the new avg
			//cpsDisplay.text += (int)cpm + "/min. ";
			
	}

	public void DoSacrifice(GameObject objHit){
				//we use insideunitsphere to get a random 3D direction and multiply the direction by our power variable
				//beamHit.rigidbody.AddForce(Random.insideUnitSphere * laserPower);
				//pulsate = true;

				//dont pulsate again before it has returned to its orig scale to prevent warping
				//if (clickOrigScale == clickable.transform.localScale){
				//Debug.Log("sacrificing... " + (1 + sacCount));
				StartCoroutine(Pulsate.Pulse(objHit, 0.15f, 0.5f, clickOrigScale));

				StartCoroutine(Radiate.oneSmoothPulse(objHit, Color.red, Color.black, 0.07f));
				audio.pitch = Random.Range(pitchMin, pitchMax);
				//audio.PlayOneShot(screams[Random.Range(0, screams.Length)]);
				int rando = (int)Random.Range(0, rumbleSound.Length);
				if (cps > 2.5f) audio.Stop();
				audio.PlayOneShot(rumbleSound[rando]);
				//sacCount++;
				//}
				GetComponent<BloodMeter>().updateMood();
				GetComponent<BloodMeter>().bloodAmt += (MasterWaypointer.vic.GetComponent<MultiSac>().multiplier * GetComponent<BloodMeter>().sacBloodValue);
				if (GetComponent<BloodMeter>().sacBloodValue < 0.25f) {
					audio.PlayOneShot(badSacClip, 0.65f); GetComponent<BloodMeter>().bloodMat.SetColor("_TintColor", Color.Lerp(GetComponent<BloodMeter>().bloodMat.GetColor("_TintColor"), GetComponent<BloodMeter>().positiveBloodColor, -0.5f));
					} else if (GetComponent<BloodMeter>().sacBloodValue > 0.25f) {
						audio.PlayOneShot(goodSacClip, 0.65f);
			GetComponent<BloodMeter>().bloodMat.SetColor("_TintColor", Color.Lerp(GetComponent<BloodMeter>().bloodMat.GetColor("_TintColor"), GetComponent<BloodMeter>().positiveBloodColor, 0.5f * GetComponent<BloodMeter>().sacBloodValue));
					}

				int multi = (int)MasterWaypointer.vic.GetComponent<MultiSac>().multiplier;
				sacCount += multi;
				//sacReady = false;
				//sacCountDisplay.text = "Total Sacrificed:	" + sacCount;
				scoreCount = sacCount - expenses;
				sacCountDisplay.text = " " + scoreCount;//Sacrifices";
				advance = true;
				if (diffManager.GetComponent<MasterWaypointer>() != null) {
					GameObject heart = Instantiate(headPrefab, diffManager.GetComponent<MasterWaypointer>().movables[0].transform.position, Quaternion.identity) as GameObject;
					var main = heart.GetComponent<ParticleSystem>().main;
					int maxx = (int)(GetComponent<BloodMeter>().sacBloodValue * 50f);
					if (maxx < 50) maxx /= 2;
					if (maxx < 20) maxx = 0;
					//Debug.Log("maxx = " + maxx);
					main.maxParticles = maxx;
					main.startSize = 3f * ((float)maxx / 50f);
					var emission = heart.GetComponent<ParticleSystem>().emission;
					emission.rate = 13f * ((float)maxx / 50f);
					diffManager.GetComponent<MasterWaypointer>().SacrificeVic();
				} else {
					Instantiate(headPrefab, objHit.transform.position + bloodOffset, Quaternion.identity);
				}
				//increase Clicks-per-minute
				//if (Time.time < startTime + cpmDuration){
				cpf++;
				//}
	}


	public void Fail(float restartTime, string failMsg){
		for (int i = 0; i < failDisable.Length; i++){
			failDisable[i].gameObject.SetActive(false);
		}
		failObj.GetComponent<Text>().text = failMsg;
		//failObj.GetComponent<Text>().fontSize = (int)(fontSize);
		Vector3 startScale = new Vector3(0.001f, 0.001f, 0.001f);
		Vector3 endScale = new Vector3(50f, 50f, 50f);
		failObj.transform.localScale = Vector3.Lerp(startScale, endScale, nLerpTime);
		nLerpTime = nLerpTime + (nLerpTime * nLerpTime);

		Drag.panMode = false;
		CraneGame.beginCraneGame = false;
		GetComponent<Drag>().panCam = Vector3.zero;
		GetComponent<CameraMove>().forceAmt = 0f;

		if (!failed){ //initiate single-call actions
			failedTime = Time.time;
			failed = true;
			//Instantiate(fireEffect, sacrificeSpot.position, Quaternion.identity);
			fireEffect.SetActive(true);

			StartCoroutine(Shake.ShakeThis(Camera.main.transform, restartTime / 10f, 0.5f));

			GameObject[] templePieces = GameObject.FindGameObjectsWithTag("particletrigger");
			foreach(GameObject temp in templePieces){
				if (temp.GetComponent<ParticleSystem>() != null) temp.GetComponent<ParticleSystem>().Play();
				if (temp.GetComponents<BoxCollider>() != null)	{
					foreach (Component comp in temp.GetComponents<BoxCollider>()){
						//Destroy(comp);
					}
				}
				//foreach (Component comp in comps) comp.enabled = false;
				//temp.AddComponent(typeof(Rigidbody));
			}
		}

		if (failedTime + restartTime < Time.time){
			//if (easyMode){
			//	easyMode = false;
				GameObject.Find("GameOverPanel").GetComponent<EndGame>().enabled = true;
			//}
			//failObj.GetComponent<Text>().text = failMsg;

		} else {

			//Camera.main.transform.DetachChildren();
			int rando = (int)Random.Range(0, rumbleSound.Length);
			//audio.loop = true;
			if (!easyMode){
				Camera.main.cullingMask = 0001111111;
				for (int i = 0; i < 5; i++){
					audio.clip = rumbleSound[rando];
					if (audio.isPlaying) audio.PlayOneShot(rumbleSound[rando]);
					if (!audio.isPlaying) audio.Play();

				}
				audio2.clip = loseClip;
				audio2.Stop();
				audio2.Play();
			}
			easyMode = true;
			//Debug.Log("FAILED restarting in... " + (restartTime - Time.time));
		}
	}

}