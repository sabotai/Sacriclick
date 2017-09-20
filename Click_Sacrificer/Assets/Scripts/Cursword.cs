using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursword : MonoBehaviour {

	public GameObject swordObj;
	public bool raycastMethod = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (raycastMethod){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit, 1000f)){
                swordObj.transform.position = new Vector3(rayHit.point.x, rayHit.point.y, swordObj.transform.position.z);
            }

            } else {
            	Vector3 scaledMouse = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, swordObj.transform.position.z);
            	scaledMouse = Vector3.Scale(scaledMouse, new Vector3 ( 13f, 13, 0f));
            	swordObj.transform.position = scaledMouse;



            }
	}
}
