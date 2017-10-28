using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sacrifice : MonoBehaviour {

	public GameObject headPrefab;
	public GameObject clickable;
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
	public bool easyMode = false;
	public bool limitAvailSac = true;
	public bool sacReady = true;
	public int sacCount = 0;
	public bool failed = false;
	float failedTime = 0.0f;
	GameObject failObj;
	public bool playScreams = false;


	// Use this for initialization
	void Start () {
		clickOrigScale = clickable.transform.localScale;
		swordOrigScale = sword.transform.localScale;
		sun = GameObject.Find("Sun");
		audio = GetComponent<AudioSource>();
		audio2 = GetComponent<AudioSource>();
		cpm = 0;
		cps = 0;
		startTime = Time.time;

		sacCountDisplay.text = sacCount + "";
		cpsDisplay.text = cpm/60 + " /second";
		failObj = GameObject.Find("GameOver");
		failObj.GetComponent<Text>().text = "";

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.P)) playScreams = !playScreams;
		if (GetComponent<Drag>().panMode){
			clickable.GetComponent<MeshRenderer>().material.color = Color.black;
		} 
		cps = cpm/60f;
		if (Input.GetKeyDown(KeyCode.E)) easyMode = !easyMode;
		
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
		if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("Default"))){


			if (beamHit.collider.gameObject == clickable) {
				sword.GetComponent<Cursword>().hideCursor = true;
				sword.GetComponent<Cursword>().currentSize *= 0.9f;
			} else {
				sword.GetComponent<Cursword>().hideCursor = false;
			}

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

				if (beamHit.collider.gameObject == clickable){
					if (!limitAvailSac || sacReady){ //if limited, check if ready
						if (!GetComponent<Drag>().panMode){
							sacReady = false;
							DoSacrifice(beamHit);
							//Debug.Log("sac reset");
						}
						
					}
				}
			}

		}


		if (Time.time >= startTime + cpmDuration){
			startTime = Time.time;
			cpm = cpm * (60/cpmDuration);
			//Debug.Log("cpm = " + cpm);
			if (sun.GetComponent<Sun>() != null)
				sun.GetComponent<Sun>().speedMult = 0.001f + cpm * cpmMag;
			cps = cpm/60f;
			//cpsDisplay.text = "Sacrifices-Per-Second:	" + cps;
			cpsDisplay.text = cps + "/sec.";
			cpm = 0f;
		}
		
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
				if (Time.time < startTime + cpmDuration){
					cpm++;
				}
	}

	public void Fail(float restartTime, string failMsg){
		failObj.GetComponent<Text>().text = failMsg;

		if (!failed){
			failedTime = Time.time;
			failed = true;

		}

		Camera.main.transform.DetachChildren();
		StartCoroutine(Shake.ShakeThis(Camera.main.transform, 10f, 0.5f));
		audio2.clip = rumbleSound;
		//audio.loop = true;
		if (audio2.isPlaying) audio2.PlayOneShot(rumbleSound);
		if (!audio2.isPlaying) audio2.Play();
		Debug.Log("FAILED restarting in... " + (restartTime - Time.time));
		if (failedTime + restartTime < Time.time){
			SceneManager.LoadScene(0);
		}
	}

}