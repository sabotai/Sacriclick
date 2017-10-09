using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sacrifice : MonoBehaviour {

	public GameObject headPrefab;
	public GameObject clickable;
	private Vector3 clickOrigScale;
	public AudioClip[] screams;
	public AudioClip rumbleSound;
	public bool advance = false;
	public Text sacCountDisplay;
	public Text cpsDisplay;
	AudioSource audio;
	private GameObject sun;
	public float cpmDuration = 5;
	public float startTime;
	public float cpmMag = 0.01f;
	public float cpm;
	public float cps;
	public bool easyMode = false;
	public bool limitAvailSac = true;
	public bool sacReady = false;
	public int sacCount = 0;


	// Use this for initialization
	void Start () {
		clickOrigScale = clickable.transform.localScale;
		sun = GameObject.Find("Sun");
		audio = GetComponent<AudioSource>();
		cpm = 0;
		cps = 0;
		startTime = Time.time;

		sacCountDisplay.text = "Total Sacrificed:	" + sacCount;
		cpsDisplay.text = "Sacrifices-Per-Second:	" + cpm/60;

	}

	// Update is called once per frame
	void Update () {
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
		if (Physics.Raycast(beam, out beamHit, 1000f)){



			//if the raycast hits a rigidbody and the player is pressing the right mouse button
			bool clicking;
			if (easyMode){
				clicking = Input.GetMouseButton(0);
			} else {
				clicking = Input.GetMouseButtonDown(0);
			}


			if (beamHit.collider.gameObject == clickable && clicking){
				if (!limitAvailSac || sacReady){ //if limited, check if ready
						DoSacrifice(beamHit);
						sacReady = false;
						Debug.Log("sac reset");
					
				}
			}

		}


		if (Time.time >= startTime + cpmDuration){
			startTime = Time.time;
			cpm = cpm * (60/cpmDuration);
			//Debug.Log("cpm = " + cpm);
			if (sun.GetComponent<Sun>() != null)
				sun.GetComponent<Sun>().speedMult = cpm * cpmMag;
			cps = cpm/60f;
			cpsDisplay.text = "Sacrifices-Per-Second:	" + cps;
			cpm = 0f;
		}
		
	}

	public void DoSacrifice(RaycastHit _beamHit){
				//we use insideunitsphere to get a random 3D direction and multiply the direction by our power variable
				//beamHit.rigidbody.AddForce(Random.insideUnitSphere * laserPower);
				//pulsate = true;

				//dont pulsate again before it has returned to its orig scale to prevent warping
				//if (clickOrigScale == clickable.transform.localScale){
				StartCoroutine(Pulsate.Pulse(_beamHit.transform.gameObject, 0.15f, 0.5f, clickOrigScale));

				StartCoroutine(Radiate.oneSmoothPulse(_beamHit.transform.gameObject, Color.red, Color.black, 0.05f));
				audio.pitch = Random.Range(0.8f, 1.2f);
				audio.PlayOneShot(screams[Random.Range(0, screams.Length)]);
				audio.PlayOneShot(rumbleSound);
				sacCount++;
				sacCountDisplay.text = "Total Sacrificed:	" + sacCount;
				//}
				Instantiate(headPrefab, _beamHit.point, Quaternion.identity);
				advance = true;
				//increase Clicks-per-minute
				if (Time.time < startTime + cpmDuration){
					cpm++;
				}
	}

}