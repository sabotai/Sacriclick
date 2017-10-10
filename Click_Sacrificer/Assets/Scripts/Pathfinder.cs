using System.Collections;
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
	public int currentWaypoint = 0;
	public Vector3 velo;
	GameObject sacrificer;
	[SerializeField] bool advance = false;
	public bool releaseDestroy = true;
	public bool auto = false;
	public bool replace = false;
	public Transform macuahuitl;
	/*[System.NonSerialized]*/ public bool imReady = false;
	public float advanceTimeOut = 1f;
	float advanceTimer = 0f;
	float origWaySpeed;
	int howMany;


	//added this because ontriggerenter was running before sacrificer was assigned
	void Awake () {
		if (macuahuitl == null) macuahuitl = GameObject.Find("sword").transform;
		if (sacrificer == null) sacrificer = GameObject.Find("Main Camera");

		if (wayParent == null) wayParent = GameObject.Find("WayParent").transform;
		if (moveSelf) movable = transform.gameObject;

		//give each one a bit of randomness for personality in movements
		waySpeed *= Random.Range(0.5f, 1.5f);
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
	
	// Update is called once per frame
	void Update () {

			if (auto || Input.GetMouseButtonDown(0)){

				//if an advance is requested from sacrifice and they are done advancing
				if (sacrificer.GetComponent<Sacrifice>().advance && !advance){
					advance = true;
					//Debug.Log("next waypoint");
					currentWaypoint++;
				}
			} else {
				
				waySpeed = origWaySpeed * (sacrificer.GetComponent<Sacrifice>().cpm + 1);
			}
		 
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
				}
			}
			


			if (isAnyoneAhead) {
				return true;
			} else {
				return false;
			}

	}
		
	
	void AutoAdvancePos(){
						movable.GetComponent<Rigidbody>().isKinematic = false;

						if (currentWaypoint < wayParent.childCount) { //if still in queue

							Vector3 target = wayParent.GetChild(currentWaypoint).position;
							Vector3 moveDirection = target - movable.transform.position;
							velo = movable.GetComponent<Rigidbody>().velocity;
					

							if (moveDirection.magnitude < 0.3f) {
								/*
								//push those forward who fell behind
								if (!advance && !sacrificer.GetComponent<Sacrifice>().isAnyoneReady){
									//if there is a hole in the sequence, move up
									
									currentWaypoint++;
									currentWaypoint = Mathf.Min(currentWaypoint, wayParent.childCount);
								}
								*/

								//bool ready = false;
								//if (currentWaypoint >= wayParent.childCount - 1) ready = true;
								//currentWaypoint++;
								/*
								for (int i = 0; i < wayParent.childCount; i++){
									if (wayParent.GetChild(i).GetComponent<Pathfinder>().currentWaypoint == wayParent.childCount - 1)
									isAnyoneReady = true;
								}
								*/
								if (currentWaypoint < wayParent.childCount - 1) {
									if (!IsAnyoneAheadOfMe()){
										currentWaypoint++;
									}
								}
								advance = false;
								//sacrificer.GetComponent<Sacrifice>().advance = false;
								//Debug.Log("Waypoint -> " + currentWaypoint);

							} else {
								//bounce by waySpeed amt?
								velo = moveDirection.normalized * waySpeed; //sets the new direction
							}

							movable.GetComponent<Rigidbody>().velocity = velo;
							moveDirection = target - movable.transform.position;
						} else { //do this after it exceeds the total
							imReady = false;
							if (transform.childCount > 0){
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
										Vector3 point = wayParent.GetChild(0).position;
										GameObject newVic = Instantiate(gameObject, point, Quaternion.identity);
										newVic.name += " " + currentWaypoint;
										newVic.transform.SetParent(gameObject.transform.parent);
										newVic.GetComponent<Pathfinder>().currentWaypoint = 1;
									}

									if (releaseDestroy){ //destroy doughnut hole
										
										gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 10f);
										GetComponent<MeshRenderer> ().material.color = Color.red;
										//destroy doughnut hole
										Destroy(transform.GetChild(0).gameObject);
										//Destroy(GetComponent<Pathfinder>());
										if (auto){

											currentWaypoint += 999;
											//Destroy(transform.GetChild(0).gameObject);
											transform.parent = GameObject.Find("trashBin").transform;
										}
									}
								}
							}
					}

	}

	void OnTriggerStay(Collider other){
		if (other.name == "PedestalZone"){
			if (currentWaypoint >= wayParent.childCount - 1){
				Debug.Log("sac ready");
				sacrificer.GetComponent<Sacrifice>().sacReady = true;
				//imReady = true;
			}
		}

	}

}
