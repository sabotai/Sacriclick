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
	//Quaternion origFingerRot;
	//public Quaternion fingerDestRot;
	public bool graspAttempted = false;
	public float gripPct = 0f;
	public bool autoRotate = false;
	public Transform lowestHeightObj;

	// Use this for initialization
	void Start () {
		defaultHeight = clawMechanism.transform.position.y;
		craneMechanism = gameObject;
		initialRotation = Camera.main.transform.rotation;
		lowestHeight = lowestHeightObj.position.y;
		//origFingerRot = clawMechanism.transform.GetChild(0).GetChild(0).localRotation;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		lowerClaw();
		rotateCrane();
		gripClaw();
	}

	void lowerClaw(){

		//LOWER CLAW

		if ((Input.GetKeyDown(downKey) || Input.GetMouseButtonDown(0)) && Mathf.Approximately(clawMechanism.transform.position.y, defaultHeight)) goingDown = true;

		if (Input.GetKey(downKey) || goingDown == false || Input.GetMouseButton(0)){
			int dir = 1;
			if (!goingDown) dir = -1;
			clawVert += (dir * clawVSpeed);
			Vector3 origin = new Vector3(clawMechanism.transform.position.x, defaultHeight, clawMechanism.transform.position.z);
			Vector3 destination = new Vector3(clawMechanism.transform.position.x, lowestHeight, clawMechanism.transform.position.z);
			clawMechanism.transform.position = Vector3.Lerp(origin, destination, clawVert);
		} 
		if (Input.GetKeyUp(downKey) || Input.GetMouseButtonUp(0)) goingDown = false;

	}

	void rotateCrane(){

		//ROTATE CRANE

		//only rotate crane in 1 direction, after grasp was attempted, and after claw returned vert
		if (MapKeys.xMovement > craneRotPct  || autoRotate){
			float rotAmt = craneRotPct;
			if (graspAttempted && Mathf.Approximately(clawMechanism.transform.position.y, defaultHeight)) {
				Camera.main.transform.rotation = Quaternion.Slerp(initialRotation, basketRot.rotation, rotAmt);
				if (autoRotate) craneRotPct += .003f;
			}

		}
		if (!autoRotate) craneRotPct = MapKeys.xMovement;

	}

	void gripClaw(){
		//GRIP CLAW

		gripPct = Mathf.Abs(MapKeys.gripOpenDelta) * gripDir;
		if (totalRotated < -31f){
			//totalRotated = -31f;
			Debug.Log("grip maximum tight");
			if (!graspAttempted){
				gripPct = 0 - gripPct;
				graspAttempted = true;
			} else {
				gripPct = 0f;
			}

		}
		if (!Input.anyKey && graspAttempted) {
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
		for(int i = 0; i < clawMechanism.transform.GetChild(0).childCount; i++){
			Transform thisFinger = clawMechanism.transform.GetChild(0).GetChild(i);
			thisFinger.Rotate(gripPct, 0f, 0f);
		}

		totalRotated += gripPct;

	}
}
