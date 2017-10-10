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
	public float randomness = 0.0f;
	public int currentWaypoint = 0;
	public Vector3 velo;
	GameObject sacrificer;
	[SerializeField] bool advance = false;
	public bool releaseDestroy = true;
	public bool auto = false;
	public bool replace = false;
	public Transform macuahuitl;
	[System.NonSerialized] public bool imReady = false;
	public float advanceTimeOut = 1f;
	float advanceTimer = 0f;
	float origWaySpeed;
	int howMany;
	public GameObject prefab;
	[System.NonSerialized]public int myCount = 0;


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
				//update the speed in between clicks
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
	bool IsAnyoneBesideMe(){
			bool isAnyoneBeside = false; // true = stay still
			GameObject victimParent;
			int sibIndex = this.transform.GetSiblingIndex();
			//Debug.Log("sibindex = " + sibIndex);
			victimParent = gameObject.transform.parent.gameObject; //find my parent
			GameObject[] victimz = new GameObject[victimParent.transform.childCount]; //setup victimz array with space for each child
			for (int i = 0; i < victimz.Length; i++){ //assign each one
				victimz[i] = victimParent.transform.GetChild(i).gameObject;
			}
			int cnt = 0; //use count so it doesnt count itself
			foreach (GameObject vic in victimz){
				if (vic.GetComponent<Pathfinder>().currentWaypoint == currentWaypoint){ //is there one ahead of me?
					cnt++;
					//Debug.Log("someone is ready");
				}

				if (cnt > 1)	isAnyoneBeside = true;
			}
			
			if (isAnyoneBeside) {
				return true;
			} else {
				return false;
			}

	}
	
	void AutoAdvancePos(){
						movable.GetComponent<Rigidbody>().isKinematic = false;

						if (currentWaypoint < wayParent.childCount) { //if still in queue

							Vector3 target = wayParent.GetChild(currentWaypoint).position;
							Vector3 moveDistance = target - movable.transform.position;
							velo = movable.GetComponent<Rigidbody>().velocity;
					

							if (moveDistance.magnitude < 0.3f) { //if it is close enough to the target waypoint
								
								
								if (currentWaypoint < wayParent.childCount - 1) {
									if (!IsAnyoneAheadOfMe()){ //notice a vacancy in the line?  move up!
										currentWaypoint++;
									}
									if (IsAnyoneBesideMe()){ //fix the ones that were getting clumped
										//Destroy(gameObject); //destroy was making the number of available vics shrink over time
										if (currentWaypoint > 0){
										 currentWaypoint--;
										 } else {
										 	currentWaypoint++;
										 }
									}
								}
								advance = false;
								
							} else {
								//bounce by waySpeed amt?
								velo = moveDistance.normalized * waySpeed; //sets the new direction
							}

							movable.GetComponent<Rigidbody>().velocity = velo;

						} else { //do this after it exceeds the total ... meaning it has been sacrificed
							imReady = false; //i am not ready anymore ... dont allow it to count as a sacrificee
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
										Vector3 point = wayParent.GetChild(0).position;
										GameObject newVic = Instantiate(prefab, point, Quaternion.identity);
										int myNewNumber = sacrificer.GetComponent<Sacrifice>().sacCount + currentWaypoint;
										Debug.Log("instantiating #" + myNewNumber);
										newVic.name += " " + (myNewNumber);
										GameObject label = newVic.transform.GetChild(1).gameObject;
										if (label.GetComponent<TextMesh>() != null) label.GetComponent<TextMesh>().text = "#" + myNewNumber;
										newVic.GetComponent<Pathfinder>().myCount = myNewNumber;
										newVic.GetComponent<Pathfinder>().currentWaypoint = 1;
										newVic.transform.SetParent(gameObject.transform.parent);
									}

									if (releaseDestroy){ //destroy doughnut hole
										
										gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 100f);
										GetComponent<MeshRenderer> ().material.color = Color.red;
										//destroy doughnut hole
										Destroy(transform.GetChild(0).gameObject);
										Destroy(transform.GetChild(1).gameObject);
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
