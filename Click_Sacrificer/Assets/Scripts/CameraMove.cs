using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

	public Transform nearFocus;
	public Transform farFocus;
	public float farZoomAmt = 10.41f;
	public float closeZoomAmt = 2f;
	private float oldZoom;

	// Use this for initialization
	void Start () {
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
	}
	
	// Update is called once per frame
	void Update () {

		float newZoom = Camera.main.GetComponent<Sacrifice>().cpm / 5f;
		float zoom = Mathf.Lerp(oldZoom, newZoom, 0.0075f);
		Debug.Log("zoomAmt = " + zoom);
		Vector3 focus = Vector3.Slerp(nearFocus.position, farFocus.position, zoom);
		gameObject.GetComponent<Camera>().orthographicSize = Mathf.Lerp(closeZoomAmt, farZoomAmt, zoom);
		transform.LookAt(focus);

		oldZoom = zoom;
	}
}
