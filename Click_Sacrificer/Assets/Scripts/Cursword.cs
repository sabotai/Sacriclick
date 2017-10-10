using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursword : MonoBehaviour {

	public GameObject swordObj;
	public bool raycastMethod = false;
	public Vector3 offset = new Vector3(0f,0f,0f);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (raycastMethod){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit, 1000f,  LayerMask.GetMask("sword-ui"))){
            	//Debug.Log("sword to ... " + rayHit.transform.gameObject.name);
                swordObj.transform.position = new Vector3(rayHit.point.x, rayHit.point.y, rayHit.point.z) + offset;
            }

        } else {
            	Vector3 scaledMouse = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, swordObj.transform.position.z);
            	scaledMouse = Vector3.Scale(scaledMouse, new Vector3 ( 13f, 13, 0f));
            	swordObj.transform.localPosition = scaledMouse + offset;



        }
	}
}
