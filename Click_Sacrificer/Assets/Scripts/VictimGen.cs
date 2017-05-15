using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimGen : MonoBehaviour {

	public int howMany = 10;
	public GameObject victimPrefab;	
	public Transform wayParent;
	public Transform[] wayChildren; 
	GameObject[] victims;

	// Use this for initialization
	void Start () {
		if (wayParent != null) {
			wayChildren	= new Transform[wayParent.childCount];
			for (int i = 0; i < wayParent.childCount; i++){
				wayChildren[i] = wayParent.GetChild(i);

			}
		}
		victims = new GameObject[howMany];
		for (int i = 0; i < victims.Length; i++){
			float pct = ((float)i/(float)victims.Length) * wayChildren.Length;
			//which node to start at
			int node = (int)(pct);
			//how far to go in between the nodes
			float intra = (pct) - (float)node;
			Debug.Log("#" + i + " ; pct = " + pct + " ; node = " + node + " ; intra = " + intra);
			if (node + 1 < wayChildren.Length){ //starting at 0 vs 1
				//start them in between the nodes
				Vector3 point = Vector3.Lerp (wayChildren[node].position, wayChildren[node + 1].position, intra);
				victims[i] = Instantiate(victimPrefab, point, Quaternion.identity);
				GameObject newPoint = new GameObject();
				newPoint.name = "NewWaypoint-" + i;
				newPoint.transform.position = point;
				//throw the new one inside the wayParent to be tracked in Pathfinder
				newPoint.transform.SetParent(wayParent);
				//reset currentwaypoint to new position based on larger magnitude
				victims[i].GetComponent<Pathfinder>().currentWaypoint = i;
				//victims[i].GetComponent><Pathfinder>().
			}
		}

		//destroy the originals so the sequence isnt messed up
		for (int i = 0; i < wayChildren.Length; i++){
			Destroy(wayChildren[i].gameObject);

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
