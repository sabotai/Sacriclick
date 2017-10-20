using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimGenToo : MonoBehaviour {

	public int howMany = 10;
	public GameObject victimPrefab;	
	public Transform wayParent;
	public GameObject[] wayChildren; 
	GameObject[] victims;
	public GameObject victimBox;
	GameObject[] wayFinalChildren;
	public Vector3 defaultRotation;

	// Use this for initialization
	void Start () {
		if (wayParent != null) {
			wayChildren	= new GameObject[wayParent.childCount];
			for (int i = 0; i < wayParent.childCount; i++){
				wayChildren[i] = wayParent.GetChild(i).gameObject;
			}
		}

		wayFinalChildren = new GameObject[howMany];
		//spawn this many victims in between the nodes
		//needs 1 extra at the front to exist in between
		if (victimBox == null) victimBox = GameObject.Find("Victims");
		if (victims == null) victims = new GameObject[howMany];

		for (int i = 0; i < howMany; i++){
			float pct = ((float)i/(float)victims.Length) * wayChildren.Length;
			//which node to start at
			int node = (int)(pct);
			//how far to go in between the nodes
			float intra = (pct) - (float)node;
			//Debug.Log("#" + i + " ; pct = " + pct + " ; node = " + node + " ; intra = " + intra);
			if (node < wayChildren.Length - 1){ //starting at 0 vs 1
				//start them in between the nodes
				Vector3 point = Vector3.Lerp (wayChildren[node].transform.position, wayChildren[node + 1].transform.position, intra);
				
				GameObject newPoint = new GameObject();
				newPoint.name = "NewWaypoint-" + i;
				newPoint.transform.position = point;
				//throw the new one inside the wayParent to be tracked in Pathfinder
				newPoint.transform.SetParent(wayParent);
				//reset currentwaypoint to new position based on larger magnitude
				//victims[i].GetComponent><Pathfinder>().
				wayFinalChildren[i] = newPoint;
			} else {

				Vector3 point = Vector3.Lerp (wayChildren[node].transform.position, wayChildren[node].transform.position, intra);
				
				GameObject newPoint = new GameObject();
				newPoint.name = "NewWaypoint-" + i;
				newPoint.transform.position = point;
				//throw the new one inside the wayParent to be tracked in Pathfinder
				newPoint.transform.SetParent(wayParent);
				//reset currentwaypoint to new position based on larger magnitude
				//victims[i].GetComponent><Pathfinder>().

				wayFinalChildren[i] = newPoint;
			}

		}

		//destroy the originals so the sequence isnt messed up
		
		for (int i = 0; i < wayChildren.Length; i++){
			Destroy(wayChildren[i]); //wayChildren are the old ones
		}
		
		Debug.Log("wayParent ChildCount = " + wayParent.childCount);
		GameObject[] wwayChildren= new GameObject[wayParent.childCount];
			for (int i = 0; i < wayParent.childCount; i++){
				wwayChildren[i] = wayParent.GetChild(i).gameObject;
			}


		for (int i = 0; i < victims.Length; i++){
				//start them in between the nodes
				Vector3 point = wayFinalChildren[i].transform.position;
				victims[i] = Instantiate(victimPrefab, point, Quaternion.Euler(defaultRotation));
				victims[i].name = "InitVictim " + (victims.Length - i);
				victims[i].transform.GetChild(1).gameObject.GetComponent<TextMesh>().text += (victims.Length - i);
				victims[i].transform.SetParent(victimBox.transform);
				
				//reset currentwaypoint to new position based on larger magnitude
				victims[i].GetComponent<Pathfinder>().currentWaypoint = i;
				//victims[i].GetComponent><Pathfinder>().
			
		}



	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
