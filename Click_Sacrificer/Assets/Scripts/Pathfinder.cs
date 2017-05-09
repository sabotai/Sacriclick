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
	int currentWaypoint;
	public Vector3 velo;

	// Use this for initialization
	void Start () {
		currentWaypoint = 0;
		if (moveSelf) movable = transform.gameObject;
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
		if (Input.GetMouseButton(0)){
			AutoAdvancePos();
		}
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
								//Debug.Log ("current toothbrush waypoint = " + currentWaypoint);
								Vector3 target = wayParent.GetChild(currentWaypoint).position;
								Vector3 moveDirection = target - movable.transform.position;
								if (moveDirection.magnitude > 0.3f){
									velo = movable.GetComponent<Rigidbody>().velocity;
					
									if (moveDirection.magnitude < 0.3f) {
											currentWaypoint++;

									} else {
											velo = moveDirection.normalized * waySpeed;
									}

									movable.GetComponent<Rigidbody>().velocity = velo;
									moveDirection = target - movable.transform.position;
								} else {
									
									movable.GetComponent<Rigidbody>().isKinematic = true;
								}
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
