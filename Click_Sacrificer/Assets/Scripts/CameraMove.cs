using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

	public Transform startFocus;
	public Transform endFocus;
	public float startZoomAmt = 38f;
	public static float endZoomAmt = 75f;
	public float oldZoom;
	public bool perspectiveCam = true;
	public bool forceShift = false;
	public float forceAmt = 0.0f;
	public Camera[] chainedCams;
	public bool zoomable = false;
	public float zoom = 0f;
	public float initialWideZoom = 120f;
	public static float currentEndZoom;
	public bool initialFlyIn = true;
	

	// Use this for initialization
	void Start () {
		currentEndZoom = initialWideZoom;
		/*
		Ray beam = Camera.main.ScreenPointToRay(Input.mousePosition);

		//draw our debug ray so we can see inside unity
		//the ray starts from beam.origin and is drawn to 1000 units in the rays direction (beam.direction)
		Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.red);

		//declare and initialize our raycasthit to store hit information
		RaycastHit beamHit = new RaycastHit();

		//this both casts the ray "beam" and returns true if the ray hits any collider
		//the second parameter is where our raycasthit information is stored
		//the third parameter is how far to cast the ray
		if (Physics.Raycast(beam, out beamHit, 1000f)){
			Debug.Log("hit " + beamHit.transform.gameObject);
		}
		*/
		oldZoom = 1f; //starting %

	}
	
	void resetZoom(){


	}

	// Update is called once per frame
	void Update () {
		if (GameState.state > 0){
			if (initialFlyIn){
				//initial fly in zoom
				if (zoom < initialWideZoom){
					if (currentEndZoom > endZoomAmt){
						currentEndZoom -= (Time.deltaTime * 10f);
					} else {
						initialFlyIn = false;
					}
				}
			} else {
				currentEndZoom = endZoomAmt;
			}


			//calculate the cpm / 5
			float newZoom = Camera.main.GetComponent<Sacrifice>().cps / 5f;

			//lerp between these 2 zoom amounts by 0.75% each frame
			zoom = Mathf.Lerp(oldZoom, newZoom, 0.0075f);
			forceAmt = Mathf.Clamp(forceAmt, 0f, 1f);
			zoom += forceAmt;
			zoom = Mathf.Clamp(zoom, 0f, 1f); //prevent from having to return to below 1 after having been forced
			//Debug.Log("zoomAmt = " + zoom);

			//focus should find the new intermediate position based on the zoom amount
			Vector3 focus = Vector3.Slerp(startFocus.position, endFocus.position, zoom);
			if (perspectiveCam){
				gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(startZoomAmt, currentEndZoom, zoom);
				foreach (Camera cam in chainedCams){
					cam.fieldOfView = Mathf.Lerp(startZoomAmt, currentEndZoom, zoom);
				}
			} else {
				gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(startZoomAmt, currentEndZoom, zoom);
				foreach (Camera cam in chainedCams){
					cam.orthographicSize = Mathf.Lerp(startZoomAmt, currentEndZoom, zoom);
				}
			}
			transform.position = focus;

	        transform.rotation = Quaternion.Lerp(startFocus.rotation, endFocus.rotation, zoom);
			//transform.LookAt(focus);

			oldZoom = zoom;

			if (zoomable){

				float scrolled = Input.GetAxis("Mouse ScrollWheel") * -10f;
				//startZoomAmt += scrolled;
				endZoomAmt += scrolled;
			}
		}
	}
}
