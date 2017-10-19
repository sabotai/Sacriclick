using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {
	GameObject dragItem;
	GameObject hoverItem;
	Color origColor;
	public Color highlightColor;
	public Material origMat;
	Vector3 panCam;
	float amtPanned = 0f;
	Transform cam;
	Transform endCam;
	public float maxPanRight;
	float velocityY = 0.0f;
	public AudioSource audio;
	public AudioClip pickup;
	public AudioClip hover;
	public AudioClip release;

	// Use this for initialization
	void Start () {
		panCam = new Vector3(0f,0f,0f);

		cam = Camera.main.gameObject.GetComponent<CameraMove>().startFocus;
		endCam = Camera.main.gameObject.GetComponent<CameraMove>().endFocus;

		maxPanRight = GameObject.Find("Victims").transform.GetChild(0).position.x;
	}
	
	// Update is called once per frame
	void Update () {

		Ray beam = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.red);
		RaycastHit beamHit = new RaycastHit();
		if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("Default"))){
			GameObject obj = beamHit.transform.gameObject;

 			if (hoverItem != null){
 				hoverItem.GetComponent<MeshRenderer> ().material.color = origMat.color;
 				hoverItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", new Color(0f,0f,0f));
 			}
 			if (obj.transform.parent != null && obj.transform.parent.name == "Victims"){
 				hoverItem = obj;
 				//audio.PlayOneShot(hover);
 				hoverItem.GetComponent<MeshRenderer> ().material.color = highlightColor;
				hoverItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", Color.white);
 				if( Input.GetMouseButtonDown(0)){
 					audio.PlayOneShot(pickup);
					dragItem = beamHit.transform.gameObject;
					panCam = Vector3.zero;
					amtPanned = 0f;
					hoverItem = null;
 					origColor = dragItem.GetComponent<MeshRenderer> ().material.color;

					//Debug.Log("GRABBED " + obj.name);
					
				}
			}

			//while dragging
			if (dragItem != null){
				dragItem.transform.position = beamHit.point;
				dragItem.layer = 2; //switch to ignore raycast
				dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", new Color(1f,1f,1f));
				dragItem.GetComponent<MeshRenderer> ().material.color = highlightColor;
						//dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EMISSION", new Color(1f,1f,1f));

				if (amtPanned >= 0 && amtPanned <= maxPanRight){
					//allow player to scroll along path
					if (Input.mousePosition.x > Screen.width * 0.9f){
							panCam += new Vector3(0.05f, 0f, 0f);
							//Debug.Log("panning right");
						} else if (Input.mousePosition.x < Screen.width * 0.1f){
							panCam -= new Vector3(0.05f, 0f, 0f);
							//Debug.Log("panning left");
						} else {
							panCam *= 0.9f;
						}
						//max pan speed in either direction
						panCam = new Vector3(Mathf.Clamp(panCam.x,-0.9f, 0.9f), 0f, 0f);

						cam.position += panCam;
						endCam.position += panCam;
						amtPanned += panCam.x;
				} else {
					//Debug.Log("pan outside border");
					//prevent it from getting stuck at either end
					//if (amtPanned < 0f)	panCam -= panCam * 2f;
					//if (amtPanned > maxPanRight) panCam -= panCam * 2f;
					panCam -= panCam * 2f;
					//amtPanned += panCam.x;
					/*
					if (amtPanned < 0f){
						amtPanned = 0f;
					} else if (amtPanned > maxPanRight) {
						amtPanned = maxPanRight;
					}
					*/

					cam.position += panCam;
					endCam.position += panCam;
					amtPanned += panCam.x;
					panCam *= 0.75f;
				}
			}

		}

		//if something is being dragged
		if (dragItem != null){
				//force the front perspective
				GetComponent<CameraMove>().forceAmt += 0.01f;

				//release
				if (Input.GetMouseButtonUp(0)){
 					audio.PlayOneShot(release);
					panCam = Vector3.zero;
					/*
					//hard reset pan position
					cam.position += new Vector3(-amtPanned, 0f, 0f);
					endCam.position += new Vector3(-amtPanned, 0f, 0f);
					panCam = Vector3.zero;
					amtPanned = panCam.x;
					*/
					dragItem.layer = 0; //switch back to default layer
					dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", new Color(0f,0f,0f));
					dragItem.GetComponent<MeshRenderer> ().material.color = origColor;
					dragItem.GetComponent<MeshRenderer> ().material.color = origMat.color;
					dragItem = null;
				}
		} else {
				//reset camera position
				GetComponent<CameraMove>().forceAmt = 0f;


				//soft reset pan position
				panCam = new Vector3(Mathf.SmoothDamp(0, -amtPanned, ref velocityY, 0.3f), 0f, 0f);
				cam.position += panCam;
				endCam.position += panCam;				
				amtPanned += panCam.x;
		}
	}
}
