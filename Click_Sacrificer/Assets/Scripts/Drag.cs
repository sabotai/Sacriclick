using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour {
	[System.NonSerialized]public GameObject dragItem;
	[System.NonSerialized]public GameObject hoverItem;
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
	public AudioClip toggleClip;
	public Color brokerModeFogColor;
	public Color bloodModeFogColor;
	GameObject diffManager;
	public GameObject pipCam;
	public GameObject pipCanvas;
	public GameObject lArrow;
	public GameObject rArrow;
	public Material pipMaterial;
	public float arrowBounceSpeed = 3f;


	// Use this for initialization
	void Start () {
		diffManager = GameObject.Find("DifficultyManager");
		panCam = new Vector3(0f,0f,0f);

		cam = Camera.main.gameObject.GetComponent<CameraMove>().startFocus;
		endCam = Camera.main.gameObject.GetComponent<CameraMove>().endFocus;

		int lastVisiblePan = 5;
		if (diffManager.GetComponent<MasterWaypointer>() != null){
			maxPanRight = GameObject.Find("Victims").transform.GetChild(GameObject.Find("Victims").transform.childCount - 1 - lastVisiblePan).position.x;
		} else {
			maxPanRight = GameObject.Find("Victims").transform.GetChild(lastVisiblePan).position.x;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (panMode){
			Ray beam = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.red);
			RaycastHit beamHit = new RaycastHit();
			if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("Default", "Background", "click-toy", "temple", "TransparentFX"))){
				GameObject obj = beamHit.transform.gameObject;


				if (hoverItem != null){ //restore hover colors
					//Debug.Log("restoring prehover colors");
					hoverItem.GetComponent<MeshRenderer> ().material.color = origColor;
					hoverItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//new Color(0f,0f,0f));
					if (hoverItem.transform.childCount > 0) {
						hoverItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = origColor;
						hoverItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//new Color(0f,0f,0f));
					}
					hoverItem = null;
				}


				if (obj.transform.parent != null && (obj.transform.parent.name == "Victims" || obj.name == "VictimLabel (1)")){ //if start hovering
					if (obj.name == "VictimLabel (1)") {
						//Debug.Log("Hovering over label");
						hoverItem = obj.transform.parent.gameObject;
					} else {
						//sup new hover item
						hoverItem = obj;
					}
					//prevent it from overriding while dragging + hovering over a new one at the same time
					if (dragItem == null){
	 					origColor = hoverItem.GetComponent<MeshRenderer> ().material.color;
	 					origEmissionColor = hoverItem.GetComponent<MeshRenderer> ().material.GetColor("_EmissionColor");
					}

	 				if( Input.GetMouseButtonDown(0)){ //init drag
	 					//panMode = true;
	 					audio.PlayOneShot(pickup);
						dragItem = hoverItem;
						hoverItem = null;
	 					//origColor = dragItem.GetComponent<MeshRenderer> ().material.color;
	 					//origEmissionColor = dragItem.GetComponent<MeshRenderer> ().material.GetColor("_EmissionColor");
					} else { //if just newly hovering
		 				audio.PlayOneShot(hover);

						//set color for both pieces of the victim
		 				hoverItem.GetComponent<MeshRenderer> ().material.color = highlightColor;
		 				hoverItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = highlightColor;
						hoverItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", highlightColor);
						hoverItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", highlightColor);
					}

				}


				//init drag colors/actions
				if (dragItem != null){
					dragItem.transform.position = beamHit.point + Vector3.up;// - beam.direction * 2f;
					dragItem.layer = 2; //switch to ignore raycast
					dragItem.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", highlightColor);
					dragItem.GetComponent<MeshRenderer> ().material.color = highlightColor;
					if (dragItem.transform.childCount > 0){
						dragItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", highlightColor);
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
					//dragItem.GetComponent<MeshRenderer> ().material.color = origMat.color;
					if (dragItem.transform.childCount > 0){
						dragItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//, new Color(0f,0f,0f));
						dragItem.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = origColor;
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


		//swapping between modes
		if (Input.GetButtonDown("Toggle") && panToggle) {
			panMode = !panMode;
			audio.PlayOneShot(toggleClip);
		}
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
					if (vic.GetComponent<Pathfinder>() != null){
						vic.GetComponent<Pathfinder>().DragInsert(relObj, vic);
					} else {
						diffManager.GetComponent<MasterWaypointer>().DragInsert(relObj, vic);
					}
					return true;
				}
			}
		}
		return false;
	}
	void doPanMode(bool yes){
		if (yes){

			lArrow.SetActive(false);
			rArrow.SetActive(false);
			//move the arrows
			//display guide arrows by default
			float originalPos = 0.5f; //default position
			float arrowOffset = Mathf.PingPong((Time.time/6f) * arrowBounceSpeed, .1f) - .1f/2f;
			//lArrow.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.left, arrowOffset, ); 
			lArrow.GetComponent<RectTransform>().pivot = new Vector2(originalPos + arrowOffset, 0f);
			//originalPos = -105f; //default position for r side

			//rArrow.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, arrowOffset); 
			rArrow.GetComponent<RectTransform>().pivot = new Vector2(originalPos - arrowOffset, 0f);

			RenderSettings.fogColor = brokerModeFogColor;
			//force the front perspective
			GetComponent<CameraMove>().forceAmt += 0.01f;

			if (amtPanned >= maxPanRight / 6f){
				//enable and fade in the pip stuff
				pipCam.GetComponent<Camera>().enabled = true;
				pipCanvas.SetActive(true);
				Color pipColor = pipMaterial.color;
				if (pipColor.a < 1f) {
					pipColor.a += 0.15f;
					pipMaterial.color = pipColor;
				}


				if (amtPanned > maxPanRight - (maxPanRight / 6f)){
					rArrow.SetActive(false);
					lArrow.SetActive(true);
				}
			} else {
			//fade out and then disable the pip stuff
				Color pipColor = pipMaterial.color;
				if (pipColor.a > 0f) {
					pipColor.a -= 0.25f;
					pipMaterial.color = pipColor;
				} else {
					pipCam.GetComponent<Camera>().enabled = false;
					pipCanvas.SetActive(false);
				}
				rArrow.SetActive(true);
				lArrow.SetActive(false);
			}

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
			//hide guide arrows
			rArrow.SetActive(false);
			lArrow.SetActive(false);

			//fade out and then disable the pip stuff
			Color pipColor = pipMaterial.color;
			if (pipColor.a > 0f) {
				pipColor.a -= 0.25f;
				pipMaterial.color = pipColor;
			} else {
				pipCam.GetComponent<Camera>().enabled = false;
				pipCanvas.SetActive(false);
			}

			RenderSettings.fogColor = bloodModeFogColor;
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
