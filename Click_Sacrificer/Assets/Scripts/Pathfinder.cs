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
	bool advance = false;

	// Use this for initialization
	void Start () {
		if (wayParent == null) wayParent = GameObject.Find("WayParent").transform;
		if (moveSelf) movable = transform.gameObject;

		if (sacrificer == null) sacrificer = GameObject.Find("Main Camera");
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
		if (Input.GetMouseButtonDown(0)){
			if (sacrificer.GetComponent<Sacrifice>().advance && !advance){
				advance = true;
				Debug.Log("next waypoint");
				currentWaypoint++;
			}
		}
			AutoAdvancePos();

	}

	void AdvancePos(){


						if (currentWaypoint < wayParent.childCount) {
								//Debug.Log ("current toothbrush waypoint = " + currentWaypoint);
								Vector3 target = wayParent.GetChild(currentWaypoint).position;
								Vector3 moveDirection = target - movable.transform.position;

								int count = 0;
								while (Vector3.Distance(target, movable.transform.position) > 0.5f){
									//velo = movable.GetComponent<Rigidbody>().velocity;
					
									//if (moveDirection.magnitude < 0.3f) {
											//currentWaypoint++;
											//timeOut = Time.time;

									//} else {
									movable.transform.position += moveDirection.normalized * Time.deltaTime;
									//}
									moveDirection = target - movable.transform.position;
									//movable.GetComponent<Rigidbody>().velocity = velo;
									Debug.Log("advancing to " + currentWaypoint + "... dist= " + Vector3.Distance(target, movable.transform.position));
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

	void AutoAdvancePos(){
						movable.GetComponent<Rigidbody>().isKinematic = false;

						if (currentWaypoint < wayParent.childCount) {
								Vector3 target = wayParent.GetChild(currentWaypoint).position;
								Vector3 moveDirection = target - movable.transform.position;
									velo = movable.GetComponent<Rigidbody>().velocity;
					
									if (moveDirection.magnitude < 0.3f) {
											//currentWaypoint++;
											advance = false;
											//sacrificer.GetComponent<Sacrifice>().advance = false;
											Debug.Log("Waypoint -> " + currentWaypoint);

									} else {
											velo = moveDirection.normalized * waySpeed;
									}

									movable.GetComponent<Rigidbody>().velocity = velo;
									moveDirection = target - movable.transform.position;
						} else {
							if(loop){
								currentWaypoint = 1;
								waySpeed *= 1.2f;
							}
						}

 /*
						if (Time.time > timeOut + 2){
							Debug.Log ("reset " + currentWaypoint);
							currentWaypoint = 1;
						}
						*/
	}
}
