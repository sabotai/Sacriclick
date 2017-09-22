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
	public float advanceTimeOut = 1f;
	float advanceTimer = 0f;


	//added this because ontriggerenter was running before sacrificer was assigned
	void Awake () {

		if (sacrificer == null) sacrificer = GameObject.Find("Main Camera");

		if (wayParent == null) wayParent = GameObject.Find("WayParent").transform;
		if (moveSelf) movable = transform.gameObject;

		//give each one a bit of randomness for personality in movements
		waySpeed *= Random.Range(0.5f, 1.5f);

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

			if (currentWaypoint == wayParent.childCount - 1) {
				sacrificer.GetComponent<Sacrifice>().isAnyoneReady = true;
				Debug.Log(this.gameObject.name + " is ready");
			}
			//if an advance is requested from sacrifice and they are done advancing
			if (sacrificer.GetComponent<Sacrifice>().advance && !advance){
				advance = true;
				//Debug.Log("next waypoint");
				currentWaypoint++;
			}
		}

		if (!advance){
			//if there is a hole in the sequence, move up
			if (!sacrificer.GetComponent<Sacrifice>().isAnyoneReady){
				//currentWaypoint++;
				currentWaypoint = Mathf.Min(currentWaypoint, wayParent.childCount);
				sacrificer.GetComponent<Sacrifice>().isAnyoneReady = true;
			}
		}
		AutoAdvancePos();
	}
/*
	void AdvancePos(){ //NOT BEING USED


						if (currentWaypoint < wayParent.childCount) {
								//Debug.Log ("current toothbrush waypoint = " + currentWaypoint);
								Vector3 target = wayParent.GetChild(currentWaypoint).position;
								Vector3 moveDirection = target - movable.transform.position;

								int count = 0;
								while (Vector3.Distance(target, movable.transform.position) > 0.5f){
	
									movable.transform.position += moveDirection.normalized * Time.deltaTime;
									
									moveDirection = target - movable.transform.position;
									//Debug.Log("advancing to " + currentWaypoint + "... dist= " + Vector3.Distance(target, movable.transform.position));
									count++;
									if (count > 1000) return; //safeguard
								}
								currentWaypoint++;
						} else {
							if(loop){
								currentWaypoint = 0;
								//waySpeed *= 1.2f;
							}
						}

	}
*/
	void AutoAdvancePos(){
						movable.GetComponent<Rigidbody>().isKinematic = false;

						if (currentWaypoint < wayParent.childCount) { //if still in queue
							//if someone is at the top of the queue, mark as ready
							Vector3 target = wayParent.GetChild(currentWaypoint).position;
							Vector3 moveDirection = target - movable.transform.position;
							velo = movable.GetComponent<Rigidbody>().velocity;
					
							if (moveDirection.magnitude < 0.3f) {
								//bool ready = false;
								//if (currentWaypoint >= wayParent.childCount - 1) ready = true;
								//currentWaypoint++;
								/*
								for (int i = 0; i < wayParent.childCount; i++){
									if (wayParent.GetChild(i).GetComponent<Pathfinder>().currentWaypoint == wayParent.childCount - 1)
									isAnyoneReady = true;
								}
								*/
								advance = false;
								//sacrificer.GetComponent<Sacrifice>().advance = false;
								//Debug.Log("Waypoint -> " + currentWaypoint);

							} else {
								//bounce by waySpeed amt?
								velo = moveDirection.normalized * waySpeed;
							}

							movable.GetComponent<Rigidbody>().velocity = velo;
							moveDirection = target - movable.transform.position;
						} else {
							if (releaseDestroy){ //destroy doughnut hole
								if (transform.childCount > 0){
									//destroy doughnut hole
									Destroy(transform.GetChild(0).gameObject);
								}
							}

							//reached the end of the line (front of the line?)
							if(loop){
								currentWaypoint = 1;
								waySpeed *= 1.2f;
							}
						}

	}

	void OnTriggerStay(Collider other){
		if (other.name == "PedestalZone"){
			//Debug.Log("sac ready");
			if (currentWaypoint == wayParent.childCount - 1)
			sacrificer.GetComponent<Sacrifice>().sacReady = true;
		}

	}
}
