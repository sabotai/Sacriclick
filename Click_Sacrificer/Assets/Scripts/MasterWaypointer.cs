using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterWaypointer : MonoBehaviour {
	Transform wayParent;
	public Transform victimParent;
	public GameObject[] movables;
	public Vector3 velo;
	public GameObject sacrificer;
	GameObject vic;
	public bool vicReady = true;
	public GameObject prefab;
	public Transform macuahuitl;
	public Vector3 spawnRotation = new Vector3(-90, 0, 180);
	public float waySpeed = 5f;
	public float randomness = 0.0f;
	float origWaySpeed;
	public static int howMany;
	bool failed = false;
	public int delayCheck = 4;
	public float maxSpeed = 4f;
	public float minSpeed = 0.5f;
	public float speedDecay = 0.9f;
	public float holdDistThresh = 0.1f;

	public float speedMultiplier = 1.1f;
	public float anxietySpeed = 0.8f;
	public Color spawnColor;
	public Color spawnEmitColor;
	public AudioClip slideClip;
	public int firstSpecialEligible = 10;
	public float specialSpawnRate = 1000;

	AudioClip myClip;
	Object[] posScreams;
	Object[] neuScreams;
	Object[] negScreams;

	float randoGen;

	//added this because ontriggerenter was running before sacrificer was assigned
	void Awake () {
		if (macuahuitl == null) macuahuitl = GameObject.Find("sword").transform;
		if (sacrificer == null) sacrificer = Camera.main.gameObject;

		if (wayParent == null) wayParent = GameObject.Find("WayParent").transform;
		
		//give each one a bit of randomness for personality in movements
		waySpeed *= Random.Range(1.0f - randomness, 1.0f + randomness);
		origWaySpeed = waySpeed;
		howMany = GameObject.Find("VictimGenerator").GetComponent<VictimGenToo>().howMany;
		failed = false;
	}

	// Use this for initialization
	void Start () {
		failed = false;
		movables = new GameObject[victimParent.childCount];
		//assign each of the positions
		for (int i = 0; i < howMany; i++){
			movables[i] = victimParent.GetChild(i).gameObject;
		}

		randoGen = Random.value;

		posScreams = Resources.LoadAll("Screams/positive", typeof(AudioClip));
		negScreams = Resources.LoadAll("Screams/negative", typeof(AudioClip));
		neuScreams = Resources.LoadAll("Screams/neutral", typeof(AudioClip));
		//Debug.Log(screams.Length + " screams");
		// print("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
		//audio = GetComponent<AudioSource>();
		//myClip = (AudioClip)neuScreams[Random.Range(0, neuScreams.Length)];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!CraneGame.beginCraneGame){
			InitRandomSpecial();


			bool releaseVics = false;
			if(failed) {
				releaseVics = true;
				if (randoGen < 0.8f){
					if (!sacrificer.GetComponent<Inventory>().failed) sacrificer.GetComponent<Sacrifice>().Fail(2f, "NEVER SACRIFICE WITHOUT CONSENT");// "You sacrificed someone without their consent!");
				} else {

					if (!sacrificer.GetComponent<Inventory>().failed) sacrificer.GetComponent<Sacrifice>().Fail(2f, "SENO EKTO GAMAT!");// "You sacrificed someone without their consent!");

				}
			} else {
				if (!sacrificer.GetComponent<BloodMeter>().failed && !sacrificer.GetComponent<Sacrifice>().failed){
					Advance();
				} else {
					releaseVics = true;
				}
			}
			if (releaseVics){
				//hide the labels
				failed = true;
				GameObject[] labels = GameObject.FindGameObjectsWithTag("label");

				foreach (GameObject label in labels){
					label.SetActive(false);
				}

				//UpdateOrder();
				for (int i  = 0; i < movables.Length; i++){
					ReleaseVic(movables[i]);			
				}
			}
		}
	}

	void InitRandomSpecial(){
		if (Time.frameCount % specialSpawnRate == 0){
			movables[Random.Range(firstSpecialEligible, movables.Length)].transform.GetChild(1).gameObject.GetComponent<SpecialStatus>().specialStat = true;
		}
	}

	void Advance(){
		UpdateOrder();
		foreach (GameObject mover in movables){
			int myIndex = mover.transform.GetSiblingIndex();
				if (myIndex == 0) {
					vic = mover;
					//Debug.Log("found our vic");
					vicReady = true;
				}
				//Debug.Log("myIndex= " + myIndex);
				Vector3 target = wayParent.GetChild(myIndex).position;
				Vector3 moveDistance = target - mover.transform.position;
				velo = mover.GetComponent<Rigidbody>().velocity;


				//cap the speed
				if (waySpeed > origWaySpeed) {
						waySpeed = Mathf.Clamp(waySpeed, minSpeed, maxSpeed);
						waySpeed *= speedDecay;
				} else {
					waySpeed = origWaySpeed;
				}
				
				if (moveDistance.sqrMagnitude < holdDistThresh) { //roughly reached destination ... bounce around a bit
					velo *= speedDecay;
					//for the front vic, allow ready if in place
					if (myIndex == 0) sacrificer.GetComponent<Sacrifice>().sacReady = true;
				} else {

					waySpeed = waySpeed * (anxietySpeed + (moveDistance.sqrMagnitude * (sacrificer.GetComponent<Sacrifice>().cps * speedMultiplier + 0.8f)));
					//Debug.Log("sqrMag = " + moveDistance.sqrMagnitude + " wayspeed= " + waySpeed);
					waySpeed = Mathf.Clamp(waySpeed, minSpeed, 100f);
					AudioSource audio = mover.GetComponent<AudioSource>();
					float pitch = Mathf.Clamp(waySpeed / 12f, 0.5f, 1.5f);
					if (waySpeed > 10f) {
						//Debug.Log("pitch= " + pitch);
						audio.pitch = pitch;//Random.Range(0.5f, 1.5f);
						audio.PlayOneShot(slideClip);
						
					} else {
					audio.pitch = 1f;
					}
					velo = moveDistance.normalized * waySpeed; //sets the new direction
				}
				mover.GetComponent<Rigidbody>().velocity = velo;

			
		}
	}

	public void SacrificeVic(){


		if (vic.transform.GetSiblingIndex() == 0 && vic.name != "dumb-idol-placeholder"){ //protection against it sac'ing the same one twice
			if (vic.transform.GetChild(1).GetComponent<SpecialStatus>().specialStat){
				CraneGame.beginCraneGame = true;
			}
			//Debug.Log("SACRIFICING: " + vic.name);
			//StartCoroutine(Shake.ShakeThis(macuahuitl, 0.6f, 0.2f));

			//reached the end of the line (front of the line?)

			//purge the child

			

			//destroy doughnut hole
			//Destroy(transform.GetChild(0).gameObject);
			//game failed if you sacrificed one who did not conset
			float myDeathMood = vic.GetComponent<Mood>().mood;
			float mt = vic.GetComponent<Mood>().moodFailThresh;


			if (Sacrifice.playScreams){
				if (myDeathMood < mt){
					Debug.Log("sacrificed someone without consent! " + myDeathMood + "  " + vic.name);
					myClip = (AudioClip)negScreams[Random.Range(0, negScreams.Length)];
					if (sacrificer.GetComponent<BloodMeter>().failureAllowed) failed = true;
				} else if (myDeathMood >= mt && myDeathMood <= Mathf.Abs(mt)) { //middle moods
					myClip = (AudioClip)neuScreams[Random.Range(0, neuScreams.Length)];
				} else { //pos moods
					myClip = (AudioClip)posScreams[Random.Range(0, posScreams.Length)];
				}
				
				AudioSource audio = vic.GetComponent<AudioSource>();
				audio.pitch = Random.Range(0.8f, 1.2f);
				audio.PlayOneShot(myClip);

			} else {
				if (myDeathMood < mt){
					if (sacrificer.GetComponent<BloodMeter>().failureAllowed) failed = true;
				}
				
			}

			ReleaseVic(vic);
			MoveUp();
			//instantiate the new one
			SpawnReplacement();
		}
	}

	public void DragInsert(GameObject swap, GameObject swap_){
		//Debug.Log("player drag: swapping " + swap.name + " for " + swap_.name);
		int swapSibCnt = swap.transform.GetSiblingIndex();
		int swap_SibCnt = swap_.transform.GetSiblingIndex();
		swap.transform.SetSiblingIndex(swap_SibCnt);
		swap_.transform.SetSiblingIndex(swapSibCnt);
	}

	public void ThrowInsert(GameObject swap, GameObject swap_){
		//Debug.Log("player drag: swapping " + swap.name + " for " + swap_.name);
		int swapSibCnt = swap.transform.GetSiblingIndex();
		int swap_SibCnt = swap_.transform.GetSiblingIndex();

		for (int i = swapSibCnt; i < swap_SibCnt; i++){
			swap_.transform.parent.GetChild(i + 1).SetSiblingIndex(i);
		}
		swap.transform.SetParent(swap_.transform.parent);
		swap.transform.SetSiblingIndex(swap_SibCnt);
	}

	public void UpdateOrder(){
		for (int i = victimParent.childCount - 1; i >= 0; i--){
			movables[i] = victimParent.GetChild(i).gameObject;
		}
	}


	void SpawnReplacement(){

		Vector3 point = wayParent.GetChild(wayParent.childCount - 1).position;
		GameObject newVic = Instantiate(prefab, point, Quaternion.Euler(spawnRotation));
		int myNewNumber = sacrificer.GetComponent<Sacrifice>().sacCount + howMany;
		//Debug.Log("instantiating #" + myNewNumber);
		newVic.name = "VicClone " + (myNewNumber);
		GameObject label = newVic.transform.GetChild(1).gameObject;
		if (label.GetComponent<TextMesh>() != null) {
			label.GetComponent<TextMesh>().text = "#" + myNewNumber;
			label.GetComponent<UpdateLabel>().label = "#" + myNewNumber;
		}

		
		newVic.transform.SetParent(victimParent);
		newVic.transform.SetAsLastSibling();
		movables[movables.Length -1] = newVic;

	}

	void ReleaseVic(GameObject releaseMe){ 

		releaseMe.GetComponent<Rigidbody>().freezeRotation = false;
		releaseMe.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 1000f);
		releaseMe.GetComponent<MeshRenderer> ().material.color = Color.red;


		if (!failed){ //only complete death if not failed
			releaseMe.layer = LayerMask.NameToLayer("Ignore Raycast");
			//defreeze statue body to break into pieces
			if (releaseMe.transform.GetChild(2) != null){
				Destroy(releaseMe.transform.GetChild(2).gameObject);
			}
			if (releaseMe.transform.GetChild(1) != null){
				Destroy(releaseMe.transform.GetChild(1).gameObject);
			}
			if (releaseMe.transform.GetChild(0) != null){
				releaseMe.transform.GetChild(0).gameObject.GetComponent<CapsuleCollider> ().isTrigger = false;
				releaseMe.transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
				releaseMe.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				releaseMe.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().isKinematic = false;
				releaseMe.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
				releaseMe.transform.GetChild(0).parent = GameObject.Find("trashBin").transform;
			}

			releaseMe.transform.parent = GameObject.Find("trashBin").transform;
			Destroy(releaseMe.GetComponent<Pathfinder>());
			Destroy(releaseMe.GetComponent<Mood>());
			Destroy(releaseMe.GetComponent<CheckSwordHover>());
		}
	}

	void MoveUp(){
		GameObject[] temp = movables;
		for(int i = 0; i < movables.Length - 1; i++){
			movables[i] = temp[i+1];
		}
	}


}
