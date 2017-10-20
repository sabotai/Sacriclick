﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

	public Transform wayParent;
    //GameObject[] waypoints;
	public string waypointPrefix;
	public bool moveSelf = false;
	public bool loop = false;
	public GameObject movable;
	public float waySpeed = 1f;
	public float randomness = 0.0f;
	public int currentWaypoint = 0;
	public Vector3 velo;
	GameObject sacrificer;
	[SerializeField] bool advancing = false;
	public bool releaseDestroy = true;
	public bool auto = false;
	public bool replace = false;
	public Transform macuahuitl;
	public Vector3 spawnRotation;
	//[System.NonSerialized] public bool imReady = false;
	public float advanceTimeOut = 1f;
	float advanceTimer = 0f;
	float origWaySpeed;
	int howMany;
	public GameObject prefab;
	[System.NonSerialized]public int myCount = 0;
	AudioSource audio;
	AudioClip myClip;
	Object[] screams;

	//added this because ontriggerenter was running before sacrificer was assigned
	void Awake () {
		if (macuahuitl == null) macuahuitl = GameObject.Find("sword").transform;
		if (sacrificer == null) sacrificer = GameObject.Find("Main Camera");

		if (wayParent == null) wayParent = GameObject.Find("WayParent").transform;
		if (moveSelf) movable = transform.gameObject;
		//give each one a bit of randomness for personality in movements
		waySpeed *= Random.Range(1.0f - randomness, 1.0f + randomness);
		origWaySpeed = waySpeed;
		howMany = GameObject.Find("VictimGenerator").GetComponent<VictimGenToo>().howMany;

		/*
		waypoints = new GameObject[wayParent.transform.childCount];

		for (int i = 0; i < waypoints.Length; i++){
			waypoints[i] = GameObject.Find(waypointPrefix + " (" + i + ")");
			waypoints[0] = GameObject.Find(waypointPrefix);
		}
		*/
	}

	void Start(){

		if (myCount == 0) myCount = transform.parent.childCount - currentWaypoint;
		//screams = Resources.Load("/screams") as AudioClip;
		screams = Resources.LoadAll("Screams", typeof(AudioClip));
		Debug.Log(screams.Length + " screams");
		// print("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
		audio = GetComponent<AudioSource>();
		myClip = (AudioClip)screams[Random.Range(0, screams.Length)];
		audio.pitch = Random.Range(0.8f, 1.2f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

			//if (auto || Input.GetMouseButtonDown(0)){

				//if an advance is requested from sacrifice and this current one is done advancing
				if (sacrificer.GetComponent<Sacrifice>().advance && !advancing){ 
					advancing = true;
					//Debug.Log("next waypoint");
					currentWaypoint++;
				}
				/*
			} else {
				//update the speed in between clicks
				waySpeed = origWaySpeed * (sacrificer.GetComponent<Sacrifice>().cpm + 1);
			}
			*/
		 
		AutoAdvancePos();
	}


	bool IsAnyoneAheadOfMe(){
			bool isAnyoneAhead = false; // true = stay still
			GameObject victimParent;
			int sibIndex = this.transform.GetSiblingIndex();
			//Debug.Log("sibindex = " + sibIndex);
			victimParent = gameObject.transform.parent.gameObject; //find my parent
			GameObject[] victimz = new GameObject[victimParent.transform.childCount]; //setup victimz array with space for each child
			for (int i = 0; i < victimz.Length; i++){ //assign each one
				victimz[i] = victimParent.transform.GetChild(i).gameObject;
			}
			foreach (GameObject vic in victimz){
				if (vic.GetComponent<Pathfinder>().currentWaypoint == currentWaypoint + 1){ //is there one ahead of me?
					isAnyoneAhead = true;
					//Debug.Log("someone is ready");
					if (vic.GetComponent<Pathfinder>().myCount > myCount){ //if they should be behind me, reorder
						SwapOrder(vic, gameObject);
					}
				}

				if (vic.GetComponent<Pathfinder>().currentWaypoint == currentWaypoint - 1){ //is there one behind of me?
					if (vic.GetComponent<Pathfinder>().myCount < myCount){ //if so, should they be in front of me?
						SwapOrder(vic, gameObject);
					}
				}
			}
			if (isAnyoneAhead) {
				return true;
			} else {
				return false;
			}

	}
	void FixAnyoneBesideMe(){
		bool isAnyoneBeside = false; // true = stay still
		GameObject victimParent;
		int sibIndex = this.transform.GetSiblingIndex();
		//Debug.Log("sibindex = " + sibIndex);
		victimParent = gameObject.transform.parent.gameObject; //find my parent
		GameObject[] victimz = new GameObject[victimParent.transform.childCount]; //setup victimz array with space for each child
		for (int i = 0; i < victimz.Length; i++){ //assign each one
			victimz[i] = victimParent.transform.GetChild(i).gameObject;
		}
		foreach (GameObject vic in victimz){
			if (vic.GetComponent<Pathfinder>().currentWaypoint == currentWaypoint){ //is there one ahead of me?
				if (vic != gameObject){
					//Debug.Log("Found a dupe!");
					if (vic.GetComponent<Pathfinder>().myCount > myCount){
							if (currentWaypoint < wayParent.childCount - 4) currentWaypoint++; //protected because weird clumping at sacpedestal
					} 

				}
			}

		}
		

	}
	
	void SwapOrder(GameObject swap, GameObject swap_){ //use to fix order when it gets off due to clumping
		int temp = swap.GetComponent<Pathfinder>().currentWaypoint;
		swap.GetComponent<Pathfinder>().currentWaypoint = swap_.GetComponent<Pathfinder>().currentWaypoint;
		swap_.GetComponent<Pathfinder>().currentWaypoint = temp;
	}

	void CleanOrder(){
		if (currentWaypoint < wayParent.childCount - 1) {
			if (!IsAnyoneAheadOfMe()){ //notice a vacancy in the line?  move up!
				currentWaypoint++;
			}
			FixAnyoneBesideMe();
		}
	}

	void AutoAdvancePos(){
		//movable.GetComponent<Rigidbody>().isKinematic = false;

		
		if (waySpeed > origWaySpeed) {
				waySpeed = Mathf.Clamp(waySpeed, 0.5f, 3f);
				waySpeed *= 0.9f;
		} else {
			waySpeed = origWaySpeed;
		}

		if (currentWaypoint < wayParent.childCount) { //if still in queue

			Vector3 target = wayParent.GetChild(currentWaypoint).position;
			Vector3 moveDistance = target - movable.transform.position;
			velo = movable.GetComponent<Rigidbody>().velocity;
	
			//if it is close enough to the target waypoint ... this is what allows gravity to effect the vic for a second before it gets recontrolled by the script (the bouncing effect)
			if (moveDistance.sqrMagnitude < 0.2f) { 
				velo *= 0.95f;
				//Debug.Log("mag less than threshold");
				//fix order in various ways
				CleanOrder();


				//if close enough, and the final pos
				if (currentWaypoint == wayParent.childCount - 1) advancing = false;

			} else {
				//bounce by waySpeed amt?

				//waySpeed = origWaySpeed + (moveDistance.magnitude * ((sacrificer.GetComponent<Sacrifice>().cpm + 1) * 7f));
				waySpeed *= (1f + (moveDistance.sqrMagnitude * (sacrificer.GetComponent<Sacrifice>().cpm + 1)));
				//Debug.Log("wayspeed= " + waySpeed);
				waySpeed = Mathf.Clamp(waySpeed, 0.5f, 100f);
				velo = moveDistance.normalized * waySpeed; //sets the new direction
			}

			movable.GetComponent<Rigidbody>().velocity = velo;

		} else { //do this after it exceeds the total ... meaning it has been sacrificed
			//imReady = false; //i am not ready anymore ... dont allow it to count as a sacrificee
			if (transform.childCount > 0){ //use the child count to prevent it from running again
				StartCoroutine(Shake.ShakeThis(macuahuitl, 0.6f, 0.2f));

				//reached the end of the line (front of the line?)
				if(loop){
					currentWaypoint = 1;
					waySpeed *= 1.2f;
				} else {
					//purge the child

				//instantiate the new one
				if (replace){
					//Debug.Log("instantiating new replacement victim");
					SpawnReplacement();
				}

				if (releaseDestroy){ //destroy doughnut hole
					audio.PlayOneShot(myClip);

					gameObject.GetComponent<Rigidbody>().freezeRotation = false;
					gameObject.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 1000f);
					GetComponent<MeshRenderer> ().material.color = Color.red;
					//destroy doughnut hole
					//Destroy(transform.GetChild(0).gameObject);

					//defreeze statue body to break into pieces
					if (transform.GetChild(1) != null){
						Destroy(transform.GetChild(1).gameObject);
					}
					if (transform.GetChild(0) != null){
						transform.GetChild(0).gameObject.GetComponent<CapsuleCollider> ().isTrigger = false;
						transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
						transform.GetChild(0).gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
						transform.GetChild(0).parent = GameObject.Find("trashBin").transform;
					}
					//Destroy(GetComponent<Pathfinder>());
					transform.parent = GameObject.Find("trashBin").transform;
					Destroy(GetComponent<Pathfinder>());
					if (auto){

						//currentWaypoint += 999;
					}
				}
			}
		}
	}

	}

	void SpawnReplacement(){

		Vector3 point = wayParent.GetChild(0).position;
		GameObject newVic = Instantiate(prefab, point, Quaternion.Euler(spawnRotation));
		int myNewNumber = sacrificer.GetComponent<Sacrifice>().sacCount + currentWaypoint;
		//Debug.Log("instantiating #" + myNewNumber);
		newVic.name = "VicClone " + (myNewNumber);
		GameObject label = newVic.transform.GetChild(1).gameObject;
		if (label.GetComponent<TextMesh>() != null) label.GetComponent<TextMesh>().text = "#" + myNewNumber;
		newVic.GetComponent<Pathfinder>().myCount = myNewNumber;
		newVic.GetComponent<Pathfinder>().currentWaypoint = 1;
		newVic.transform.SetParent(gameObject.transform.parent);
	}

	void OnTriggerStay(Collider other){
		if (other.name == "PedestalZone"){
			if ((currentWaypoint == wayParent.childCount - 1) && !advancing){
				//Debug.Log("sac ready");
				sacrificer.GetComponent<Sacrifice>().sacReady = true;
				//imReady = true;
			}
		}

	}

}
