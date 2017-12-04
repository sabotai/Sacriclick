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
	float craneRotPct = 0f;
	public Transform basketRot;
	public float gripDir = -1f;
	public float totalRotated = 0f;
	Quaternion origFingerRot;
	public Quaternion fingerDestRot;
	bool graspAttempted = false;
	public float gripPct = 0f;

	// Use this for initialization
	void Start () {
		defaultHeight = clawMechanism.transform.position.y;
		craneMechanism = gameObject;
		initialRotation = Camera.main.transform.rotation;
		lowestHeight = GameObject.Find("Lowest").transform.position.y;
		origFingerRot = clawMechanism.transform.GetChild(0).GetChild(0).localRotation;
	}
	
	// Update is called once per frame
	void Update () {
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

		//only rotate crane in 1 direction, after grasp was attempted, and after claw returned vert
		if (MapKeys.xMovement > craneRotPct && graspAttempted && clawVert == 0f) {
			Camera.main.transform.rotation = Quaternion.Slerp(initialRotation, basketRot.rotation, MapKeys.xMovement);
		}
		craneRotPct = MapKeys.xMovement;
		/*
		if (totalRotated < 30f * (1f - MapKeys.gripOpenDelta) && MapKeys.gripOpenDelta != 0){
			Vector3 rotDegs = Vector3.left * Time.deltaTime * 100f;
			totalRotated += rotDegs.x;
			for(int i = 0; i < clawMechanism.transform.GetChild(0).childCount; i++){
				clawMechanism.transform.GetChild(0).GetChild(i).Rotate(rotDegs);
			}
		} 
		*/

		gripPct = Mathf.Abs(MapKeys.gripOpenDelta) * gripDir;
		if (!Input.anyKey && totalRotated != 0f) {
			gripPct = Mathf.Abs(gripPct);
			Debug.Log("player released claw");
		}
		if (totalRotated < -31f){
			//they released keys
			gripDir = 1f;
			Debug.Log("reopenning grip");
			graspAttempted = true;

		}
		if (totalRotated > 1f) {
			gripPct = 0f;
		}

		if (gripPct != 0f) Debug.Log("gripPct = " + gripPct);
			/*
			if (Mathf.Abs(totalRotated) > 31f || totalRotated < 0f){
				gripDir *= -1;
				Debug.Log("reversing grip dir");
			}
			*/
				for(int i = 0; i < clawMechanism.transform.GetChild(0).childCount; i++){
					Transform thisFinger = clawMechanism.transform.GetChild(0).GetChild(i);
					//fingerDestRot = Quaternion.Euler(thisFinger.localEulerAngles.x, 30f, origFingerRot.eulerAngles.z);

					thisFinger.Rotate(gripPct, 0f, 0f);
					//thisFinger.localRotation = Quaternion.Slerp(origFingerRot, fingerDestRot, gripPct);

				}

				totalRotated += gripPct;



		/*
		//auto grasp
		if (grasp) {
			if (Mathf.Abs(totalRotated) < 32f){
				Vector3 rotDegs = Vector3.left * Time.deltaTime * 100f;
				totalRotated += rotDegs.x;
				for(int i = 0; i < clawMechanism.transform.GetChild(0).childCount; i++){
					clawMechanism.transform.GetChild(0).GetChild(i).Rotate(rotDegs);
				}
			} 

		}
		*/
	}
}
