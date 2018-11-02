using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Drag : MonoBehaviour {
	[System.NonSerialized]public GameObject dragItem;
	[System.NonSerialized]public GameObject hoverItem;
	[System.NonSerialized]public GameObject flickItem;
	//GameObject vicParent;
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
	public AudioSource audio, audioInd;
	public AudioClip pickup;
	public AudioClip hover;
	public AudioClip goodRelease;
	public AudioClip badRelease;
	public static bool panMode = false;
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
	public GameObject placeholderItem;
	Vector3 pMouse, mouseVelo;
	public float flickThresh = 30f;
	public float flickForce = 4000f;
	public AudioClip flickClip;
	public float dragZoomAmt = 49f;
	float origEndZoomAmt;
	public float panSpeed = 1.5f;
	public GameObject vicParent;

	// Use this for initialization
	void Start () {
		//vicParent = GameObject.Find("Victims");
		diffManager = GameObject.Find("DifficultyManager");
		panCam = new Vector3(0f,0f,0f);
		//panToggle = false;
		panMode = false;

		origEndZoomAmt = CameraMove.endZoomAmt;
		cam = Camera.main.gameObject.GetComponent<CameraMove>().startFocus;
		endCam = Camera.main.gameObject.GetComponent<CameraMove>().endFocus;

		int lastVisiblePan = 2;
		if (diffManager.GetComponent<MasterWaypointer>() != null){
			maxPanRight = vicParent.transform.GetChild(vicParent.transform.childCount - 1 - lastVisiblePan).position.x;
		} else {
			maxPanRight = vicParent.transform.GetChild(lastVisiblePan).position.x;
		}

		if (ColorblindMode.cbMode || PlayerPrefs.GetInt("color") > 2){
			lArrow.GetComponent<Text>().color = ColorblindMode.cbGreen;
			rArrow.GetComponent<Text>().color = ColorblindMode.cbGreen;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!Tips.displayingTip){
		if (GameState.state == 2){
			Ray beam = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.red);
			RaycastHit beamHit = new RaycastHit();
			if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("Default", "Background", "click-toy", "temple", "TransparentFX"))){
				GameObject obj = beamHit.transform.gameObject;


				if (hoverItem != null){ //restore hover colors
					//Debug.Log("restoring prehover colors");
					resetColor(hoverItem);
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
	 					//audio.PlayOneShot(pickup);
						dragItem = hoverItem;
							hoverItem = null;
							audioInd = dragItem.GetComponent<AudioSource>();
							audioInd.Stop();
							audioInd.clip = pickup;
							audioInd.Play();
						} else { //if just newly hovering
							audioInd = hoverItem.GetComponent<AudioSource>();
							audioInd.Stop();
							audioInd.clip = hover;
							audioInd.Play();
		 				//audio.PlayOneShot(hover);

						//set color for both pieces of the victim
						setColor(hoverItem, highlightColor);
					}

				}


				//init drag colors/actions
				if (dragItem != null){
					dragItem.transform.position = beamHit.point + Vector3.up; //slight offset to prevent going through the ground

					dragItem.layer = 2; //switch to ignore raycast
					setColor(dragItem, highlightColor);
							
				}

			} //end of raycast

			//if something is being dragged
			if (dragItem != null){
					audioInd = dragItem.GetComponent<AudioSource>();
				//release
				if (Input.GetMouseButtonUp(0)){



					if (!panToggle) panMode = false;
					
					dragItem.layer = 0; //switch back to default layer
					resetColor(dragItem);

					dragFail = !insert(dragItem, false);

					if (mouseVelo.magnitude > flickThresh && flickItem == null && dragItem.transform.GetSiblingIndex() != 0){
						Debug.Log("velo = " + mouseVelo.magnitude);
						flickItem = dragItem;
						flickItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
						} else if (dragFail) {
							//audioInd.Stop();
							//audioInd.clip = badRelease;
							//audioInd.Play();
							audio.PlayOneShot(badRelease, 0.6f);
						} else {
							//audioInd.Stop();
							//audioInd.clip = goodRelease;
							//audioInd.Play();

							audio.PlayOneShot(goodRelease, 0.5f);
					}
					dragItem = null;

				}
			} else { //if dragitem is null
				if (!panToggle) panMode = false;
			}
		} else { //if !panmode
			if (GameState.state != -1){
				pipCam.GetComponent<Camera>().enabled = false;
				pipCanvas.SetActive(false);
				//reset them if the player swapped back to blood while hovering or dragging
				if (dragItem != null) resetColor(dragItem);
				if (hoverItem != null) resetColor(hoverItem);
			}
		}


		if (flickItem != null) flick(flickItem);

		if (GameState.state == 1 || GameState.state == 2){
			//swapping between modes
			if (Input.GetButtonDown("Toggle") && panToggle) {
				panMode = !panMode;
					audio.Stop();
					audio.clip = toggleClip;
					audio.Play();
			}

			 doPanMode(Input.GetButton("Toggle") || panMode);

			mouseVelo = Input.mousePosition - pMouse;
			pMouse = Input.mousePosition;
		} else {
			//panMode = false;
		}
		}

	}

	void resetColor(GameObject me){

		me.GetComponent<MeshRenderer> ().material.color = origColor;
		me.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//new Color(0f,0f,0f));
		if (me.transform.childCount > 0) {
			me.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = origColor;
			me.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", origEmissionColor);//new Color(0f,0f,0f));
		}

	}

	void setColor(GameObject me, Color thisColor){
		me.GetComponent<MeshRenderer> ().material.color = thisColor;
		me.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", thisColor);

		//this is the second piece of the victim
		me.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = thisColor;
		me.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", thisColor);

	}
	bool insert(GameObject relObj, bool isThrow){
		audioInd = relObj.GetComponent<AudioSource>();
		GameObject victimParent;
		int sibIndex = relObj.transform.GetSiblingIndex();
		//Debug.Log("sibindex = " + sibIndex);
		if (relObj.transform.parent != null){ 
			victimParent = relObj.transform.parent.gameObject; //find my parent
		} else {
			victimParent = vicParent;
		}
		GameObject[] victimz = new GameObject[victimParent.transform.childCount]; //setup victimz array with space for each child
		for (int i = 0; i < victimz.Length; i++){ //assign each one
			victimz[i] = victimParent.transform.GetChild(i).gameObject;
		}
		foreach (GameObject vic in victimz){
			if (Vector3.Distance(vic.transform.position, relObj.transform.position) < insertThresh){
				if (vic != relObj){ //prevent from swapping with itself
					Debug.Log("swapping " + sibIndex + " for " + vic.transform.GetSiblingIndex());

						if (!isThrow){
							diffManager.GetComponent<MasterWaypointer>().DragInsert(relObj, vic);
						} else {
							diffManager.GetComponent<MasterWaypointer>().ThrowInsert(relObj, vic);
						}

					return true;
				}
			}
		}
		return false;
	}

	void flick(GameObject flickee){
		Debug.Log("flicking item ... " + flickee.name);
		audioInd = flickee.GetComponent<AudioSource>();
		int sibIndex = flickee.transform.GetSiblingIndex();
		if (flickee.transform.position.x > vicParent.transform.GetChild(0).position.x && flickee.transform.position.x < (vicParent.transform.GetChild(vicParent.transform.childCount - 1).position.x) && flickee.transform.position.z < 10f && flickee.transform.position.z > 0f ){
			if (placeholderItem.transform.parent != null) {
				sibIndex = placeholderItem.transform.GetSiblingIndex(); //override it if it has already swapped
			} else {
				//audio.PlayOneShot(flickClip);
				audioInd.Stop();
				audioInd.clip = flickClip;
				audioInd.Play();
				placeholderItem.transform.SetParent(vicParent.transform);
				placeholderItem.transform.SetSiblingIndex(sibIndex);
				diffManager.GetComponent<MasterWaypointer>().movables[sibIndex] = placeholderItem;
				flickee.transform.SetParent(null);
				flickee.GetComponent<Rigidbody>().velocity = Vector3.zero; //needs to be zeroed from old return velocity
				flickee.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(mouseVelo) * flickForce);
				//flickee.GetComponent<Rigidbody>().AddForce(mouseVelo * 200f);
				Debug.Log("sending it on its way...");

			}

			if (insert(flickee, true)) { //if found a new home 
				Debug.Log("putting back flickee at " + sibIndex);
				placeholderItem.transform.SetParent(null);
				diffManager.GetComponent<MasterWaypointer>().UpdateOrder();
				flickItem = null;
			}
		} else {
			//audioInd.Stop();
			//audioInd.clip = badRelease;
			//audioInd.Play();
			audio.PlayOneShot(badRelease);
			Debug.Log("flicked item out of bounds, resetting... " );
			if (flickee.transform.parent == null){ //if it is currently parentless

				sibIndex = placeholderItem.transform.GetSiblingIndex(); //override it if it has already swapped

				//reset to original position if out of bounds
				placeholderItem.transform.parent = null;
				flickee.transform.SetParent(vicParent.transform);
				flickee.transform.SetSiblingIndex(sibIndex);
				diffManager.GetComponent<MasterWaypointer>().UpdateOrder();
			}
			flickItem = null;
		}

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
				//zoom in
				if (CameraMove.endZoomAmt > dragZoomAmt) CameraMove.endZoomAmt -= 20 * Time.deltaTime;
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
				//resetting camera position
				if (CameraMove.endZoomAmt < origEndZoomAmt) CameraMove.endZoomAmt += 20 * Time.deltaTime;

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
						panCam += new Vector3(panSpeed, 0f, 0f) * Time.deltaTime;
						//Debug.Log("panning right");
					} else if (Input.mousePosition.x < Screen.width * 0.1f){
						panCam -= new Vector3(panSpeed, 0f, 0f) * Time.deltaTime;
						//Debug.Log("panning left");
					} else {
						panCam *= 0.9f;
					}
					//max pan speed in either direction
					panCam = new Vector3(Mathf.Clamp(panCam.x,-panSpeed, panSpeed), 0f, 0f);

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
			if (CameraMove.endZoomAmt < origEndZoomAmt && !CraneGame.beginCraneGame) CameraMove.endZoomAmt += 30 * Time.deltaTime;
			
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
