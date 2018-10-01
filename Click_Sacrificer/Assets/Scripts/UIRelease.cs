using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRelease : MonoBehaviour {

	GameObject classicMe;
	public GameObject menuEnable;
	public GameObject menuDisable;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ReleaseMe(){
		classicMe = Instantiate(gameObject, transform.parent);
		classicMe.SetActive(false);
		GetComponent<Rigidbody>().freezeRotation = false;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
	}

	public void ReleaseLastChild(){
		GameObject target = transform.GetChild(transform.childCount - 1).gameObject;
		GetComponent<Button>().enabled = false;
		classicMe = Instantiate(target, transform);
		classicMe.SetActive(false);
		target.GetComponent<Rigidbody>().freezeRotation = false;
		target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
	}

	public void OnTriggerEnter(Collider other){
		if (other.tag == "menufall"){
			transform.parent.gameObject.GetComponent<Button>().enabled = true;
			if (menuEnable)		menuEnable.SetActive(true);
			if (menuDisable)menuDisable.SetActive(false);
			if (classicMe) classicMe.SetActive(true);
			transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject.SetActive(true);
			gameObject.SetActive(false);
		}
	}
}
