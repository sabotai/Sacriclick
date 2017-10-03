using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {
	GameObject dragItem;
	GameObject hoverItem;
	Color origColor;
	public Color highlightColor;
	public Material origMat;

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

 			if (hoverItem != null){
 				hoverItem.GetComponent<MeshRenderer> ().material.color = origMat.color;
 				hoverItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", new Color(0f,0f,0f));
 			}
 			if (obj.transform.parent != null && obj.transform.parent.name == "Victims"){
 				hoverItem = obj;
 				hoverItem.GetComponent<MeshRenderer> ().material.color = highlightColor;
				hoverItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", Color.white);
 				if( Input.GetMouseButtonDown(0)){
					dragItem = beamHit.transform.gameObject;
					hoverItem = null;
 					origColor = dragItem.GetComponent<MeshRenderer> ().material.color;

					Debug.Log("GRABBED " + obj.name);
					
				}
			}

			if (dragItem != null){
				dragItem.transform.position = beamHit.point;
				dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", new Color(1f,1f,1f));
				dragItem.GetComponent<MeshRenderer> ().material.color = highlightColor;
						//dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EMISSION", new Color(1f,1f,1f));
			}

		}
		if (dragItem != null){


				//release
				if (Input.GetMouseButtonUp(0)){
					dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", new Color(0f,0f,0f));
					dragItem.GetComponent<MeshRenderer> ().material.color = origColor;
					dragItem.GetComponent<MeshRenderer> ().material.color = origMat.color;
					dragItem = null;
				}
		}
	}
}
