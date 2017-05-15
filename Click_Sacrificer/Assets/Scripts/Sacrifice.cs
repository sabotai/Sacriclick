using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sacrifice : MonoBehaviour {

	public GameObject headPrefab;
	public GameObject clickable;
	private Vector3 clickOrigScale;
	public AudioClip[] screams;
	public bool advance = false;
	public Text sacCountDisplay;
	public Text cpsDisplay;
	AudioSource audio;
	private GameObject sun;
	public float cpmDuration = 5;
	public float startTime;
	public float cpmMag = 0.01f;
	float cpm;


	public int sacCount = 0;

	// Use this for initialization
	void Start () {
		clickOrigScale = clickable.transform.localScale;
		sun = GameObject.Find("Sun");
		audio = GetComponent<AudioSource>();
		cpm = 0;
		startTime = Time.time;

		sacCountDisplay.text = "Sacrifices:  " + sacCount;
		cpsDisplay.text = "Clicks-Per-Second:  " + cpm/60;
	}

	// Update is called once per frame
	void Update () {
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
			if (beamHit.collider.gameObject == clickable && Input.GetMouseButtonDown(0)){
				//we use insideunitsphere to get a random 3D direction and multiply the direction by our power variable
				//beamHit.rigidbody.AddForce(Random.insideUnitSphere * laserPower);
				//pulsate = true;

				//dont pulsate again before it has returned to its orig scale to prevent warping
				//if (clickOrigScale == clickable.transform.localScale){
				StartCoroutine(Pulsate.Pulse(beamHit.transform.gameObject, 0.15f, 0.5f, clickOrigScale));
				audio.pitch = Random.Range(0.8f, 1.2f);
				audio.PlayOneShot(screams[Random.Range(0, screams.Length)]);
				sacCount++;
				sacCountDisplay.text = "Sacrifices:  " + sacCount;
				//}
				Instantiate(headPrefab, beamHit.point, Quaternion.identity);
				advance= true;
				//increase Clicks-per-minute
				if (Time.time < startTime + cpmDuration){
					cpm++;
				}
			}

		}


		if (Time.time >= startTime + cpmDuration){
			startTime = Time.time;
			cpm = cpm * (60/cpmDuration);
			Debug.Log("cpm = " + cpm);
			sun.GetComponent<Sun>().speedMult = cpm * cpmMag;

			cpsDisplay.text = "Clicks-Per-Second:  " + cpm/60;
			cpm = 0f;
		}
	}

}