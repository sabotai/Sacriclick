using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Presets;

public class AltTemple : MonoBehaviour {

	public bool altTemple = true;
	//public CameraMove camMover;
	public CameraMove altCam, normCam;
	public GameObject[] enableObjs;
	public GameObject[] disableObjs;
	public Sprite daggerSprite;
	public MasterWaypointer master;
	public GameObject altPrefab;
	public GameObject[] altCleanRemains;
	// Use this for initialization
	void Awake () {
		if (Random.value > 0.5f) altTemple = true; else altTemple = false;
		if (altTemple){
			master.prefab = altPrefab;
			master.cleanRemains = altCleanRemains;
			//Destroy(normCam);
			//CameraMove newMover = Instantiate<CameraMove>(altCam, Camera.main.transform);
			//Debug.Log("newMover = " + newMover.gameObject.name);
			//altCam.ApplyTo(camMover);
			normCam.startFocus = altCam.startFocus;
			normCam.endFocus = altCam.endFocus;
			normCam.startZoomAmt = altCam.startZoomAmt;
			normCam.startFocus = altCam.startFocus;


			foreach(GameObject enableObj in enableObjs){
				enableObj.SetActive(true);
			}
			foreach(GameObject disableObj in disableObjs){
				disableObj.SetActive(false);
			}

			Camera.main.transform.position = normCam.startFocus.position;
			Camera.main.transform.rotation = normCam.startFocus.rotation;

			GameObject.Find("sword").GetComponent<SpriteRenderer>().sprite = daggerSprite;

		} else {
			Destroy(altCam);

			foreach(GameObject enableObj in enableObjs){
				enableObj.SetActive(false);
			}
			foreach(GameObject disableObj in disableObjs){
				disableObj.SetActive(true);
			}

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
