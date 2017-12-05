using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour {
	public KeyCode downKey;
	public float lowestHeight;
	float defaultHeight; 
	Quaternion initialRotation;
	public GameObject clawMechanism;
	GameObject craneMechanism;
	bool goingDown = true;
	public float clawVSpeed = .01f;
	float clawVert;
	public float craneRotPct = 0f;
	public Transform basketRot;
	public float gripDir = -1f;
	public float totalRotated = 0f;
	Quaternion[] origFingerRot;
	//public Quaternion fingerDestRot;
	public bool graspAttempted = false;
	public float gripPct = 0f;
	public bool autoRotate = false;
	public Transform lowestHeightObj;
	public float maxGrip = -31f;
	float startTime;
	public float clawHoldDuration = 1f;
	public bool completed = false;


	void Awake(){
		origFingerRot = new Quaternion[clawMechanism.transform.GetChild(0).childCount];
		for(int i = 0; i < clawMechanism.transform.GetChild(0).childCount; i++){
			origFingerRot[i] = clawMechanism.transform.GetChild(0).GetChild(i).localRotation;
		}
	}
	// Use this for initialization
	public void Start () {
		Debug.Log("initializing claw");
		defaultHeight = clawMechanism.transform.position.y;
		craneMechanism = gameObject;
		initialRotation = GameObject.Find("Camera Sub").transform.rotation;
		lowestHeight = lowestHeightObj.position.y;
		craneRotPct = 0f;
		totalRotated = 0f;
		gripDir = -1f;
		graspAttempted = false;
		goingDown = true;
		completed = false;
		clawVert = 0f;
		startTime = -1f;

		for(int i = 0; i < clawMechanism.transform.GetChild(0).childCount; i++){
			clawMechanism.transform.GetChild(0).GetChild(i).localRotation = origFingerRot[i];
		}
	}



	// Update is called once per frame
	void FixedUpdate () {
		lowerClaw();
		rotateCrane();
		gripClaw();

		if (craneRotPct >= 0.99f){
			completed = true;
		}
	}

	void lowerClaw(){

		//LOWER CLAW

		if ((Input.GetKeyDown(downKey) || 
			Input.GetMouseButtonDown(0)) && 
			Mathf.Approximately(clawMechanism.transform.position.y, defaultHeight)) {

				goingDown = true;
			}

		if (Input.GetKey(downKey) || goingDown == false || Input.GetMouseButton(0)){
			int dir = 1;
			if (!goingDown) {
				//lets reverse it if no longer going down
				dir = -1;
				graspAttempted = true;
			}
			clawVert += (dir * clawVSpeed);
			Vector3 origin = new Vector3(clawMechanism.transform.position.x, defaultHeight, clawMechanism.transform.position.z);
			Vector3 destination = new Vector3(clawMechanism.transform.position.x, lowestHeight, clawMechanism.transform.position.z);
			clawMechanism.transform.position = Vector3.Lerp(origin, destination, clawVert);
		} 

		if (Mathf.Approximately(clawVert, 1f) && startTime < 0f) startTime = Time.time;
		if (Input.GetKeyUp(downKey) || Input.GetMouseButtonUp(0) || 
			(Time.time > startTime + clawHoldDuration && startTime > 0f)) {
			goingDown = false;
			startTime = -1f;
		}

	}

	void rotateCrane(){

		//ROTATE CRANE

		//only rotate crane in 1 direction, after grasp was attempted, and after claw returned vert
		if (MapKeys.xMovement > craneRotPct  || autoRotate){
			float rotAmt = craneRotPct;
			if ((graspAttempted) && Mathf.Approximately(clawMechanism.transform.position.y, defaultHeight)) {
				Debug.Log("ROTATING CRANE");
				Camera.main.transform.rotation = Quaternion.Slerp(initialRotation, basketRot.rotation, rotAmt);
				if (autoRotate) craneRotPct += .003f;
				//if (Mathf.Approximately(rotAmt, 1f)) startTime = Time.time;

			}

		}
		if (!autoRotate) craneRotPct = MapKeys.xMovement;

	}

	void gripClaw(){
		//GRIP CLAW

		gripPct = Mathf.Abs(MapKeys.gripOpenDelta) * gripDir;
		if (totalRotated < maxGrip){ //if gripping max tight
			//totalRotated = -31f;
			Debug.Log("grip maximum tight");
			if (!graspAttempted){
				//reverse slightly
				gripPct = 0 - gripPct;

				graspAttempted = true;
			} else {
				gripPct = 0f;
			}

		}

		//player completely released grip 
		if ((!Input.anyKey && graspAttempted) || completed) { //autorelease for completed
			gripPct = Mathf.Abs(gripPct);
			Debug.Log("player released claw");

			//they released keys
			gripDir = 1f;
			gripPct = 0.2f;
			Debug.Log("reopenning grip");

		}

		if (totalRotated > 1f) {
			gripPct = 0f;
			Debug.Log("grip maximum loose");
			graspAttempted = true;
		}

		if (gripPct != 0f) Debug.Log("gripPct = " + gripPct);
		//use gripPct to rotate each finger
		for(int i = 0; i < clawMechanism.transform.GetChild(0).childCount; i++){
			Transform thisFinger = clawMechanism.transform.GetChild(0).GetChild(i);
			thisFinger.Rotate(gripPct, 0f, 0f);
		}

		totalRotated += gripPct;

	}
}
