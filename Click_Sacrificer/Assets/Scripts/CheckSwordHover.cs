using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSwordHover : MonoBehaviour {

	bool swordHovering = false;
	public float moodHoverValue = 0.1f;
	public float labelOrigY;
	// Use this for initialization
	void Start () {
		labelOrigY = transform.GetChild(2).localPosition.y;
		
	}
	
	// Update is called once per frame
	void Update () {
			Vector3 camDir = Vector3.Normalize(Camera.main.transform.position - transform.position);
			Ray ray = new Ray(transform.position, camDir);
			RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit, 1000f,  LayerMask.GetMask("sword"))){
            	swordHovering = true;
            } else {
            	swordHovering = false;
            }

            if(swordHovering) {
            	gameObject.GetComponent<Mood>().moodDir = 1f;
            	transform.GetChild(2).position += new Vector3(0f, Mathf.PingPong(Time.time / 1.5f, 0.1f) - 0.05f, 0f); //bounce label
				//StartCoroutine(Pulsate.Pulse(gameObject.transform.GetChild(2).gameObject, 0.15f, 0.5f));
            	
            } else {
            	//reset label position
            	transform.GetChild(2).localPosition = new Vector3(transform.GetChild(2).localPosition.x, labelOrigY, transform.GetChild(2).localPosition.z);
            }

		
	}
}
