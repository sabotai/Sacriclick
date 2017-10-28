using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {
	GameObject dragItem;
	GameObject hoverItem;
	Color origColor;
	public Color highlightColor;
	Color origEmissionColor;
	public Material origMat;
	public Vector3 panCam;
	public float insertThresh = 0.5f;
	float amtPanned = 0f;
	Transform cam;
	Transform endCam;
	public float maxPanRight;
	float velocityY = 0.0f;
	public AudioSource audio;
	public AudioClip pickup;
	public AudioClip hover;
	public AudioClip goodRelease;
	public AudioClip badRelease;
	public bool panMode = false;
	public bool panToggle = true;
	bool dragFail = false;


	// Use this for initialization
	void Start () {
		panCam = new Vector3(0f,0f,0f);

		cam = Camera.main.gameObject.GetComponent<CameraMove>().startFocus;
		endCam = Camera.main.gameObject.GetComponent<CameraMove>().endFocus;

		maxPanRight = GameObject.Find("Victims").transform.GetChild(0).position.x;
	}
	
	// Update is called once per frame
	void Update () {

		if (panMode){
			Ray beam = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.red);
			RaycastHit beamHit = new RaycastHit();
			if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("Default"))){
				GameObject obj = beamHit.transform.gameObject;

	 			if (hoverItem != null){ //if hovering already
	 				hoverItem.GetComponent<MeshRenderer> ().material.color = origMat.color;
	 				hoverItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//new Color(0f,0f,0f));
	 				if (hoverItem.transform.childCount > 0) {
		 				hoverItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = origMat.color;
		 				hoverItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//new Color(0f,0f,0f));
		 			}
	 			}
	 			if (obj.transform.parent != null && obj.transform.parent.name == "Victims"){ //if start hovering
	 				hoverItem = obj;
	 				origColor = hoverItem.GetComponent<MeshRenderer> ().material.color;
	 				origEmissionColor = hoverItem.GetComponent<MeshRenderer> ().material.GetColor("_EmissionColor");

	 				if( Input.GetMouseButtonDown(0)){
	 					panMode = true;
	 					audio.PlayOneShot(pickup);
						dragItem = beamHit.transform.gameObject;
						hoverItem = null;
	 					//origColor = dragItem.GetComponent<MeshRenderer> ().material.color;
	 					//origEmissionColor = dragItem.GetComponent<MeshRenderer> ().material.GetColor("_EmissionColor");
					} else {
		 				//audio.PlayOneShot(hover);
		 				hoverItem.GetComponent<MeshRenderer> ().material.color = highlightColor;
		 				hoverItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = highlightColor;
						hoverItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", Color.white);
						hoverItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", Color.white);
					}

				}

				//while dragging
				if (dragItem != null){
					dragItem.transform.position = beamHit.point;
					dragItem.layer = 2; //switch to ignore raycast
					dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", new Color(1f,1f,1f));
					dragItem.GetComponent<MeshRenderer> ().material.color = highlightColor;
					if (dragItem.transform.childCount > 0){
						dragItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", new Color(1f,1f,1f));
						dragItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = highlightColor;
					}
							
				}

			}

			//if something is being dragged
			if (dragItem != null){
				//force the front perspective
				//GetComponent<CameraMove>().forceAmt += 0.01f;

				//release
				if (Input.GetMouseButtonUp(0)){
					if (!panToggle) 
						panMode = false;

					//panCam = Vector3.zero;
					/*
					//hard reset pan position
					cam.position += new Vector3(-amtPanned, 0f, 0f);
					endCam.position += new Vector3(-amtPanned, 0f, 0f);
					panCam = Vector3.zero;
					amtPanned = panCam.x;
					*/
					dragItem.layer = 0; //switch back to default layer
					dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//new Color(0f,0f,0f));
					dragItem.GetComponent<MeshRenderer> ().material.color = origColor;
					dragItem.GetComponent<MeshRenderer> ().material.color = origMat.color;
					if (dragItem.transform.childCount > 0){
						dragItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//, new Color(0f,0f,0f));
						dragItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = origMat.color;
					}
					dragFail = !insert(dragItem);
					if (dragFail) {
						audio.PlayOneShot(badRelease);
					} else {
						audio.PlayOneShot(goodRelease);
					}
					dragItem = null;

				}
			} else {
				if (!panToggle) panMode = false;
			}
		}

		if (Input.GetButtonDown("Toggle") && panToggle) panMode = !panMode;
		doPanMode(Input.GetButton("Toggle") || panMode);

	}
	bool insert(GameObject relObj){
		GameObject victimParent;
		int sibIndex = relObj.transform.GetSiblingIndex();
		//Debug.Log("sibindex = " + sibIndex);
		victimParent = relObj.transform.parent.gameObject; //find my parent
		GameObject[] victimz = new GameObject[victimParent.transform.childCount]; //setup victimz array with space for each child
		for (int i = 0; i < victimz.Length; i++){ //assign each one
			victimz[i] = victimParent.transform.GetChild(i).gameObject;
		}
		foreach (GameObject vic in victimz){
			if (Vector3.Distance(vic.transform.position, relObj.transform.position) < insertThresh){
				if (vic != relObj){ //prevent from swapping with itself
					Debug.Log("swapping " + sibIndex + " for " + vic.transform.GetSiblingIndex());
					vic.GetComponent<Pathfinder>().DragInsert(relObj, vic);
					return true;
				}
			}
		}
		return false;
	}
	void doPanMode(bool yes){
		if (yes){
			//force the front perspective
			GetComponent<CameraMove>().forceAmt += 0.01f;

			//if outside the boundaries
			if (amtPanned >= 0 && amtPanned <= maxPanRight){
				//allow player to scroll along path
				if (Input.mousePosition.x > Screen.width * 0.9f){
						panCam += new Vector3(1f, 0f, 0f) * Time.deltaTime;
						//Debug.Log("panning right");
					} else if (Input.mousePosition.x < Screen.width * 0.1f){
						panCam -= new Vector3(1f, 0f, 0f) * Time.deltaTime;
						//Debug.Log("panning left");
					} else {
						panCam *= 0.9f;
					}
					//max pan speed in either direction
					panCam = new Vector3(Mathf.Clamp(panCam.x,-0.9f, 0.9f), 0f, 0f);

					cam.position += panCam;
					endCam.position += panCam;
					amtPanned += panCam.x;
			} else { //if inside the boundaries ... pan the cam
				panCam -= panCam * 2f;
				cam.position += panCam;
				endCam.position += panCam;
				amtPanned += panCam.x;
				panCam *= 0.75f;
			}
		} else {
			if (dragFail)	panCam = Vector3.zero;

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
