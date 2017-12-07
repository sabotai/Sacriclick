using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganReset : MonoBehaviour {
	ParticleSystem myPS;
	GameObject clawObj;

	Vector3 origin;
	// Use this for initialization
	void Awake () {
		origin = transform.position;
		clawObj = GameObject.Find("claw-2");
		if (GetComponent<ParticleSystem>() != null) myPS = GetComponent<ParticleSystem>();
	}
	void OnEnable(){
		transform.position = origin;
	}

	// Update is called once per frame
	void Update () {
		}
	void OnCollisionEnter(Collision col){

		if (col.gameObject.tag == "claw" || col.gameObject.tag == "organ" && myPS != null && !myPS.IsAlive()){
			myPS.Play();
		}
		if (col.gameObject.tag == "Temple"){

			if (!clawObj.GetComponent<Claw>().grabbing){
				if (clawObj.GetComponent<Claw>().clawVert <= 0f && clawObj.GetComponent<Claw>().stage != 0){
					Debug.Log("crane game reset ... organ hit temple");
					Camera.main.gameObject.GetComponent<CraneGame>().beginCraneGame = false;
				}
			}
		}
	}
	
}
