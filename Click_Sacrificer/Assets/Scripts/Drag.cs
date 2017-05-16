using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {
	GameObject dragItem;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Ray beam = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.red);
		RaycastHit beamHit = new RaycastHit();
		if (Physics.Raycast(beam, out beamHit, 1000f)){
			GameObject obj = beamHit.transform.gameObject;
 			if( Input.GetMouseButtonDown(0)){
 				if (obj.transform.parent != null && obj.transform.parent.name == "Victims"){
					Debug.Log("GRABBED " + obj.name);
					dragItem = beamHit.transform.gameObject;
				}
			}

			if (dragItem != null){
				dragItem.transform.position = beamHit.point;
			}

		}
		//release
		if (Input.GetMouseButtonUp(0)){
			dragItem = null;
		}
	}
}
