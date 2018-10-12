using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRelease : MonoBehaviour {

	GameObject classicMe;
	public GameObject[] menuEnable;
	public GameObject[] menuDisable;
	public ParticleSystem bloodSys;
	GameObject diffMan;
	public bool resume = false;
	public bool quit = false;
	public bool restart = false;
	public bool openLink = false;
	public AudioClip clip, clip2;
	public GameObject[] deathComponents;
	public float timeOut = 0f;
	float startTime = 0f;

	//ParticleSystem bloodSys2;
	// Use this for initialization
	void Start () {
		diffMan = GameObject.Find("DifficultyManager");
		if (!bloodSys) bloodSys = GameObject.Find("UIBloodSys").GetComponent<ParticleSystem>();
		//bloodSys2 =  GameObject.Find("UIBloodSystem").transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void OnEnable () {

	}
	// Update is called once per frame
	void Update () {
		if (timeOut > 0f){
			if (Time.time > startTime + timeOut){
				End();
			}
		}
	}

	public void ReleaseMe(){
		classicMe = Instantiate(gameObject, transform.parent);
		classicMe.SetActive(false);
		GetComponent<Rigidbody>().freezeRotation = false;
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

	}

	public void ReleaseLastChild(){
		GameObject target = transform.GetChild(transform.childCount - 1).gameObject;
		if (GetComponent<Button>()) GetComponent<Button>().enabled = false;
		classicMe = Instantiate(target, transform);
		classicMe.SetActive(false);
		target.GetComponent<Rigidbody>().freezeRotation = false;
		target.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;		

		if (target.GetComponent<UIRelease>().resume || target.GetComponent<UIRelease>().restart || target.GetComponent<UIRelease>().quit) {
			target.GetComponent<MeshRenderer>().enabled = false;
			target.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
			Destroy(target.GetComponent<CapsuleCollider>());
			Destroy(target.transform.GetChild(0).gameObject.GetComponent<CapsuleCollider>());
			foreach (GameObject deathComponent in deathComponents){
				GameObject deadPart = Instantiate(deathComponent, target.transform);
				//deadPart.GetComponent<Rigidbody>().AddExplosionForce(15f, deadPart.transform.position - Vector3.right, 5f, 1f);

				deadPart.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 1000f);
			}
			Destroy(target.GetComponent<Rigidbody>());
			//ParticleSystem part = Instantiate(target.GetComponent<UIRelease>().bloodSys, target.transform);//GameObject.Find("UIBloodSys").
			target.GetComponent<ParticleSystem>().Play();
			if (Camera.main.GetComponent<AudioSource>()) {
				Camera.main.GetComponent<AudioSource>().PlayOneShot(target.GetComponent<UIRelease>().clip);
				Camera.main.GetComponent<AudioSource>().PlayOneShot(target.GetComponent<UIRelease>().clip2);
				//target.GetComponent<Rigidbody>().AddExplosionForce(10f, Random.insideUnitSphere, 0.1f, 3f);
			}
			target.GetComponent<UIRelease>().timeOut = 1.25f; 
		}
		target.GetComponent<UIRelease>().startTime = Time.time;
	}

	public void OnTriggerEnter(Collider other){
		//Debug.Log("collided with ... " + other.name);
		if (other.tag == "menufall"){
			bloodSys.Stop();
			bloodSys.Play();
			//if (bloodSys2) bloodSys2.Play();
			End();

		}

	}

	public void End(){
			transform.parent.gameObject.GetComponent<Button>().enabled = true;
			foreach (GameObject menuItem in menuEnable){
				menuItem.SetActive(true);
			}
			foreach (GameObject menuItem in menuDisable){
				menuItem.SetActive(false);
			}
			//if (menuDisable) menuDisable.SetActive(false);
			if (classicMe) classicMe.SetActive(true);
			transform.parent.GetChild(transform.GetSiblingIndex() + 1).gameObject.SetActive(true);
			gameObject.SetActive(false);
			if (resume){
				diffMan.GetComponent<GameState>().Resume();
			} else if (restart){
				diffMan.GetComponent<GameState>().RestartGame();
			} else if (quit){
				diffMan.GetComponent<GameState>().QuitGame();
				if (openLink) diffMan.GetComponent<OpenHyperlink>().OpenLink();
			} else {

				if (Camera.main.GetComponent<AudioSource>()) {
					Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);
					Camera.main.GetComponent<AudioSource>().PlayOneShot(clip2);
				}
			}
	}
}
