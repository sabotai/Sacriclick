using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sacrifice : MonoBehaviour {

	public GameObject headPrefab;
	GameObject clickable;
	public GameObject sword;
	Vector3 swordOrigScale;
	private Vector3 clickOrigScale;
	//public AudioClip[] screams;
	public AudioClip rumbleSound;
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
	public bool failed = false;
	float failedTime = 0.0f;
	GameObject failObj;
	public bool playScreams = false;
	public Vector3 bloodOffset;


	// Use this for initialization
	void Start () {
		easyMode = false;
		clickable = GameObject.Find("click-toy");
		clickOrigScale = clickable.transform.localScale;
		swordOrigScale = sword.transform.localScale;
		sun = GameObject.Find("Sun");
		audio = GetComponent<AudioSource>();
		audio2 = GetComponent<AudioSource>();
		cpm = 0f;
		cps = 0f;
		cpf = 0f;
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

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P)) playScreams = !playScreams;
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(0);
		if (Input.GetKeyDown("escape")) Application.Quit();
		if (Input.GetKeyDown(KeyCode.E)) easyMode = !easyMode;
		if (GetComponent<Drag>().panMode){
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

		//this both casts the ray "beam" and returns true if the ray hits any collider
		//the second parameter is where our raycasthit information is stored
		//the third parameter is how far to cast the ray
		if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("click-toy"))){

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
				Vector3 swordOrigPos = sword.transform.localPosition;
				StartCoroutine(Pulsate.PulsePos(sword.transform.gameObject, 0.15f, swordOrigScale.x / 20f, swordOrigPos));

				if (beamHit.collider.gameObject == clickable || beamHit.collider.gameObject.tag == "click-toy"){
					if (!limitAvailSac || sacReady){ //if limited, check if ready
						if (!GetComponent<Drag>().panMode){
							sacReady = false;
							if (beamHit.collider.gameObject == clickable) {
								DoSacrifice(beamHit);
							} else {
								DoSacrifice(clickable);
							}
							//Debug.Log("sac reset");
						}
						
					}
				}
			}

		}



			calcCPS();
			
		
	}

	void calcCPS(){
		//cps = cpf * Time.deltaTime;
		int fps = (int)(1f / Time.deltaTime);
		int useFrameAmt = (int)Mathf.Clamp(fps, 0f, 60f);

		//move everything down by one
		for (int i = useFrameAmt - 1; i > 0; i--){
			
			cpsSamples[i] = cpsSamples[i - 1];

			//Debug.Log(cpsSamples[i]);
		}
		cpsSamples[0] = cpf;


		float totalCps = 0f;
		for (int i = 0; i < useFrameAmt; i++){
			totalCps += cpsSamples[i];
		}
		//avgCps /= cpsSamples.Length;
		cps = totalCps;// / cpsSamples.Length; //update to the new avg
		//Debug.Log("avgCps = " + cps);
		cpf = 0f;

		//if (Time.time >= startTime + cpmDuration){
		//	startTime = Time.time;
			/*
			cps = (cpm / cpmDuration); //current cps


			cpsSamples[0] = cps;
			//move everything down by one
			for (int i = 0; i < cpsSamples.Length; i++){
				if (i < cpsSamples.Length - 1){
					cpsSamples[i+1] = cpsSamples[i];
				}
			}
			*/
			calcCPM();

			//Debug.Log("cpm = " + cpm);
			if (sun.GetComponent<SunPct>() != null && sun.GetComponent<SunPct>().manControl == false )
				sun.GetComponent<SunPct>().rotAmt = (sun.GetComponent<SunPct>().rotAmt * 0.7f) + ((cps / maxCps) * 0.3f);
			//cpsDisplay.text = "Sacrifices-Per-Second:	" + cps;
			//ppCps = pCps;
			//pCps = cps;
		//}
		cpsDisplay.text = (int)cps + "/s.  " + (int)cpm + "/m.";
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
				audio.pitch = Random.Range(0.8f, 1.2f);
				//audio.PlayOneShot(screams[Random.Range(0, screams.Length)]);
				audio.PlayOneShot(rumbleSound);
				//sacCount++;
				//}
				GetComponent<BloodMeter>().updateMood();
				GetComponent<BloodMeter>().bloodAmt += GetComponent<BloodMeter>().sacBloodValue;
				sacCount++;
				//sacReady = false;
				//sacCountDisplay.text = "Total Sacrificed:	" + sacCount;

				sacCountDisplay.text = " " + sacCount;//Sacrifices";
				Instantiate(headPrefab, objHit.transform.position + bloodOffset, Quaternion.identity);
				advance = true;
				//increase Clicks-per-minute
				//if (Time.time < startTime + cpmDuration){
					cpf++;
				//}
	}

	public void DoSacrifice(RaycastHit _beamHit){
				//we use insideunitsphere to get a random 3D direction and multiply the direction by our power variable
				//beamHit.rigidbody.AddForce(Random.insideUnitSphere * laserPower);
				//pulsate = true;

				//dont pulsate again before it has returned to its orig scale to prevent warping
				//if (clickOrigScale == clickable.transform.localScale){
				//Debug.Log("sacrificing... " + (1 + sacCount));
				StartCoroutine(Pulsate.Pulse(_beamHit.transform.gameObject, 0.15f, 0.5f, clickOrigScale));

				StartCoroutine(Radiate.oneSmoothPulse(_beamHit.transform.gameObject, Color.red, Color.black, 0.07f));
				audio.pitch = Random.Range(0.8f, 1.2f);
				//audio.PlayOneShot(screams[Random.Range(0, screams.Length)]);
				audio.PlayOneShot(rumbleSound);
				//sacCount++;
				//}
				GetComponent<BloodMeter>().updateMood();
				GetComponent<BloodMeter>().bloodAmt += GetComponent<BloodMeter>().sacBloodValue;
				sacCount++;
				//sacReady = false;
				//sacCountDisplay.text = "Total Sacrificed:	" + sacCount;

				sacCountDisplay.text = " " + sacCount;//Sacrifices";
				Instantiate(headPrefab, _beamHit.point, Quaternion.identity);
				advance = true;
				//increase Clicks-per-minute
				//if (Time.time < startTime + cpmDuration){
					cpf++;
				//}
	}

	public void Fail(float restartTime, string failMsg){
		failObj.GetComponent<Text>().text = failMsg;
		GetComponent<Drag>().panMode = false;
		GetComponent<Drag>().panCam = Vector3.zero;
		GetComponent<CameraMove>().forceAmt = 0f;

		if (!failed){
			failedTime = Time.time;
			failed = true;

			StartCoroutine(Shake.ShakeThis(Camera.main.transform, restartTime / 10f, 0.5f));
		}

		if (failedTime + restartTime < Time.time){
			
			if (Input.anyKey)	{
				easyMode = false;
				SceneManager.LoadScene(0);
			}
		} else {
			Camera.main.cullingMask = 0001111111;

			//Camera.main.transform.DetachChildren();
			easyMode = true;
			audio2.clip = rumbleSound;
			//audio.loop = true;
			if (audio2.isPlaying) audio2.PlayOneShot(rumbleSound);
			if (!audio2.isPlaying) audio2.Play();
			Debug.Log("FAILED restarting in... " + (restartTime - Time.time));
		}
	}

}