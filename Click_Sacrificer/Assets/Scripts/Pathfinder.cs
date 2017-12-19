using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

	public Transform wayParent;
    //GameObject[] waypoints;
	public string waypointPrefix;
	public bool moveSelf = false;
	public bool loop = false;
	public bool moving = false;
	public GameObject movable;
	public float waySpeed = 1f;
	public float randomness = 0.0f;
	public int currentWaypoint = 0;
	public Vector3 velo;
	GameObject sacrificer;
	[SerializeField] bool advancing = false;
	public bool releaseDestroy = true;
	public bool auto = false;
	public bool replace = false;
	public Transform macuahuitl;
	public Vector3 spawnRotation;
	bool pathfinderReady = true;
	//[System.NonSerialized] public bool imReady = false;
	public float advanceTimeOut = 1f;
	float advanceTimer = 0f;
	float origWaySpeed;
	int howMany;
	public GameObject prefab;
	[System.NonSerialized]public int myCount = 0;
	AudioSource audio;
	AudioClip myClip;
	Object[] posScreams;
	Object[] neuScreams;
	Object[] negScreams;
	bool playScreams;
	bool failed = false;
	public int delayCheck = 3;
	public float maxSpeed = 3f;
	public float minSpeed = 0.5f;
	public float speedDecay = 0.9f;
	public float holdDistThresh = 0.2f;
	public float speedMultiplier = 3f;
	public float anxietySpeed = 2f;
	public Color spawnColor;
	public Color spawnEmitColor;

	//added this because ontriggerenter was running before sacrificer was assigned
	void Awake () {
		if (macuahuitl == null) macuahuitl = GameObject.Find("sword").transform;
		if (sacrificer == null) sacrificer = GameObject.Find("Main Camera");

		if (wayParent == null) wayParent = GameObject.Find("WayParent").transform;
		if (moveSelf) movable = transform.gameObject;
		//give each one a bit of randomness for personality in movements
		waySpeed *= Random.Range(1.0f - randomness, 1.0f + randomness);
		origWaySpeed = waySpeed;
		howMany = GameObject.Find("VictimGenerator").GetComponent<VictimGenToo>().howMany;
		failed = false;
		/*
		waypoints = new GameObject[wayParent.transform.childCount];

		for (int i = 0; i < waypoints.Length; i++){
			waypoints[i] = GameObject.Find(waypointPrefix + " (" + i + ")");
			waypoints[0] = GameObject.Find(waypointPrefix);
		}
		*/
	}

	void Start(){
		playScreams = Sacrifice.playScreams;
		failed = false;
		if (myCount == 0) myCount = transform.parent.childCount - currentWaypoint;
		//screams = Resources.Load("/screams") as AudioClip;
		posScreams = Resources.LoadAll("Screams/positive", typeof(AudioClip));
		negScreams = Resources.LoadAll("Screams/negative", typeof(AudioClip));
		neuScreams = Resources.LoadAll("Screams/neutral", typeof(AudioClip));
		//Debug.Log(screams.Length + " screams");
		// print("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
		audio = GetComponent<AudioSource>();
		myClip = (AudioClip)neuScreams[Random.Range(0, neuScreams.Length)];
		audio.pitch = Random.Range(0.8f, 1.2f);

		transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", spawnEmitColor);
		transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = spawnColor;
		
		gameObject.GetComponent<MeshRenderer> ().material.SetColor("_EmissionColor", spawnEmitColor);
		gameObject.GetComponent<MeshRenderer> ().material.color = spawnColor;

	}
	
	// Update is called once per frame
	void FixedUpdate () {


		if(failed) {
			ReleaseVic();
			sacrificer.GetComponent<Sacrifice>().Fail(2f, "NEVER SACRIFICE WITHOUT CONSENT!");// "You sacrificed someone without their consent!");
		} else {
			if (!sacrificer.GetComponent<BloodMeter>().failed && !sacrificer.GetComponent<Sacrifice>().failed){



				//if an advance is requested from sacrifice and this current one is done advancing
				if (sacrificer.GetComponent<Sacrifice>().advance && !advancing){ 
					advancing = true;
					//Debug.Log("next waypoint");
					currentWaypoint++;
				}
			 
				AutoAdvancePos();
			} else {
				ReleaseVic();
			}
		}
	}


	bool IsAnyoneAheadOfMe(GameObject[] victimz){
			bool isAnyoneAhead = false; // true = stay still
			foreach (GameObject vic in victimz){
				if (vic.GetComponent<Pathfinder>().currentWaypoint == currentWaypoint + 1){ //is there one ahead of me?
					isAnyoneAhead = true;
					//Debug.Log("someone is ready");
					if (vic.GetComponent<Pathfinder>().myCount > myCount){ //if they should be behind me, reorder
						SwapOrder(vic, gameObject);
					}
				}

				if (vic.GetComponent<Pathfinder>().currentWaypoint == currentWaypoint - 1){ //is there one behind of me?
					if (vic.GetComponent<Pathfinder>().myCount < myCount){ //if so, should they be in front of me?
						SwapOrder(vic, gameObject);
					}
				}

				if (isAnyoneAhead) return true; //try to return if found so as to not cycle through everything (efficiency)
			}
			if (isAnyoneAhead) {
				return true;
			} else {
				return false;
			}

	}
	void FixAnyoneBesideMe(GameObject[] victimz){
		bool isAnyoneBeside = false; // true = stay still
		foreach (GameObject vic in victimz){
			if (vic.GetComponent<Pathfinder>().currentWaypoint == currentWaypoint){ //is there one ahead of me?
				if (vic != gameObject){ //if not this gameobject
					Debug.Log("Found a dupe!");
					if (vic.GetComponent<Pathfinder>().myCount > myCount){
							if (currentWaypoint < wayParent.childCount - 4) currentWaypoint++; //protected because weird clumping at sacpedestal
					} 

				}
			}

		}
		

	}
	
	void SwapOrder(GameObject swap, GameObject swap_){ //use to fix order when it gets off due to clumping
		//Debug.Log("clean house: swapping " + swap.name + " for " + swap_.name);
		int temp = swap.GetComponent<Pathfinder>().currentWaypoint;
		swap.GetComponent<Pathfinder>().currentWaypoint = swap_.GetComponent<Pathfinder>().currentWaypoint;
		swap_.GetComponent<Pathfinder>().currentWaypoint = temp;
	}

	public void DragInsert(GameObject swap, GameObject swap_){ //use to fix order when it gets off due to clumping
		//Debug.Log("player drag: swapping " + swap.name + " for " + swap_.name);
		int swapCnt = swap.GetComponent<Pathfinder>().myCount;
		int swapSibCnt = swap.transform.GetSiblingIndex();
		int swap_SibCnt = swap_.transform.GetSiblingIndex();
		swap.transform.SetSiblingIndex(swap_SibCnt);
		swap_.transform.SetSiblingIndex(swapSibCnt);
		swap.GetComponent<Pathfinder>().myCount = swap_.GetComponent<Pathfinder>().myCount;
		swap_.GetComponent<Pathfinder>().myCount = swap.GetComponent<Pathfinder>().myCount;
		int temp = swap.GetComponent<Pathfinder>().currentWaypoint;
		swap.GetComponent<Pathfinder>().currentWaypoint = swap_.GetComponent<Pathfinder>().currentWaypoint;
		swap_.GetComponent<Pathfinder>().currentWaypoint = temp;
	}

	void CleanOrder(){

			GameObject victimParent;
			int sibIndex = this.transform.GetSiblingIndex();
			//Debug.Log("sibindex = " + sibIndex);
			victimParent = gameObject.transform.parent.gameObject; //find my parent
			GameObject[] victimz = new GameObject[victimParent.transform.childCount]; //setup victimz array with space for each child
			for (int i = 0; i < victimz.Length; i++){ //assign each one
				victimz[i] = victimParent.transform.GetChild(i).gameObject;
			}

	
		if (currentWaypoint < wayParent.childCount - 1) {
			//previously, delayCheck was set to 4
			//trying to check with the count so it doesnt overlap
			if (Mathf.Approximately(Time.frameCount % myCount, 0f)){	
				FixAnyoneBesideMe(victimz);
			}
			if (!IsAnyoneAheadOfMe(victimz)){ //notice a vacancy in the line?  move up!
					currentWaypoint++;
			}
		} 
	}

	void AutoAdvancePos(){
		//movable.GetComponent<Rigidbody>().isKinematic = false;

		
		if (waySpeed > origWaySpeed) {
				waySpeed = Mathf.Clamp(waySpeed, minSpeed, maxSpeed);
				waySpeed *= speedDecay;
		} else {
			waySpeed = origWaySpeed;
		}

		if (currentWaypoint < wayParent.childCount) { //if still in queue

			Vector3 target = wayParent.GetChild(currentWaypoint).position;
			Vector3 moveDistance = target - movable.transform.position;
			velo = movable.GetComponent<Rigidbody>().velocity;
	
			//if it is close enough to the target waypoint ... this is what allows gravity to effect the vic for a second before it gets recontrolled by the script (the bouncing effect)
			if (moveDistance.sqrMagnitude < holdDistThresh) { //roughly reached destination ... bounce around a bit
				velo *= speedDecay;
				//Debug.Log("mag less than threshold");
				//fix order in various ways
				CleanOrder();

				moving = false;
				//if close enough, and the final pos
				//if (CheckIfReady()) advancing = false;
				advancing = false;

				



			} else { //still need to advance
				moving = true;
				//bounce by waySpeed amt?

				//waySpeed = origWaySpeed + (moveDistance.magnitude * ((sacrificer.GetComponent<Sacrifice>().cpm + 1) * 7f));
				
				waySpeed = waySpeed * (anxietySpeed + (moveDistance.sqrMagnitude * (sacrificer.GetComponent<Sacrifice>().cps * speedMultiplier + 0.8f)));
				//Debug.Log("sqrMag = " + moveDistance.sqrMagnitude + " wayspeed= " + waySpeed);
				waySpeed = Mathf.Clamp(waySpeed, minSpeed, 100f);
				velo = moveDistance.normalized * waySpeed; //sets the new direction
			}

			movable.GetComponent<Rigidbody>().velocity = velo;

		} else { //do this after it exceeds the total ... meaning it has been sacrificed
			//imReady = false; //i am not ready anymore ... dont allow it to count as a sacrificee
			if (transform.childCount > 0){ //use the child count to prevent it from running again
				StartCoroutine(Shake.ShakeThis(macuahuitl, 0.6f, 0.2f));

				//reached the end of the line (front of the line?)
				if(loop){
					currentWaypoint = 1;
					waySpeed *= 1.2f;
				} else {
					//purge the child

					//advancing = false;
					//instantiate the new one
					if (replace){
						//Debug.Log("instantiating new replacement victim");

						if (pathfinderReady) 
							SpawnReplacement();

						


					}

					if (releaseDestroy){ //destroy doughnut hole
						//destroy doughnut hole
						//Destroy(transform.GetChild(0).gameObject);
						//game failed if you sacrificed one who did not conset
						float myDeathMood = gameObject.GetComponent<Mood>().mood;
						float mt = gameObject.GetComponent<Mood>().moodFailThresh;
						if (myDeathMood < mt){
							Debug.Log("sacrificed someone without consent!");
							myClip = (AudioClip)negScreams[Random.Range(0, negScreams.Length)];
							if (sacrificer.GetComponent<BloodMeter>().failureAllowed) failed = true;
						} else if (myDeathMood >= mt && myDeathMood <= Mathf.Abs(mt)) { //middle moods
							myClip = (AudioClip)neuScreams[Random.Range(0, neuScreams.Length)];
						} else { //pos moods
							myClip = (AudioClip)posScreams[Random.Range(0, posScreams.Length)];
						}

						if (playScreams) audio.PlayOneShot(myClip);
						ReleaseVic();
						if (!failed){ //only complete death if not failed
							gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
							//defreeze statue body to break into pieces
							if (transform.GetChild(2) != null){
								Destroy(transform.GetChild(2).gameObject);
							}
							if (transform.GetChild(1) != null){
								Destroy(transform.GetChild(1).gameObject);
							}
							if (transform.GetChild(0) != null){
								transform.GetChild(0).gameObject.GetComponent<CapsuleCollider> ().isTrigger = false;
								transform.GetChild(0).gameObject.GetComponent<MeshRenderer> ().material.color = Color.red;
								transform.GetChild(0).gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
								transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
								transform.GetChild(0).parent = GameObject.Find("trashBin").transform;
							}

							transform.parent = GameObject.Find("trashBin").transform;
							Destroy(GetComponent<Pathfinder>());
							Destroy(GetComponent<Mood>());
							Destroy(GetComponent<CheckSwordHover>());
						}
					}
				}
		}
	}

	}

	void SpawnReplacement(){

		Vector3 point = wayParent.GetChild(0).position;
		GameObject newVic = Instantiate(prefab, point, Quaternion.Euler(spawnRotation));
		int myNewNumber = sacrificer.GetComponent<Sacrifice>().sacCount + currentWaypoint;
		//Debug.Log("instantiating #" + myNewNumber);
		newVic.name = "VicClone " + (myNewNumber);
		GameObject label = newVic.transform.GetChild(1).gameObject;
		if (label.GetComponent<TextMesh>() != null) label.GetComponent<TextMesh>().text = "#" + myNewNumber;
		newVic.GetComponent<Pathfinder>().myCount = myNewNumber;
		newVic.GetComponent<Pathfinder>().currentWaypoint = 1;
		newVic.transform.SetParent(gameObject.transform.parent);
		newVic.transform.SetAsFirstSibling();
	}

	void ReleaseVic(){ 

		gameObject.GetComponent<Rigidbody>().freezeRotation = false;
		gameObject.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 1000f);
		GetComponent<MeshRenderer> ().material.color = Color.red;
	}

	//maybe no longer necessary/used
	bool CheckIfReady(){
		//bool ready = true;

		//checking if it is at the front of the line
		if (currentWaypoint != wayParent.childCount - 1) return false;


		//making sure the previous spawn exists
		if (transform.GetSiblingIndex() != transform.parent.childCount - 1) return false;
		
		//making sure the last one is in the right spot (i.e., not moving (no clumping!!!))
		//if (transform.parent.GetChild(transform.parent.childCount - 1).gameObject.GetComponent<Pathfinder>().moving) ready = false;

		if (transform.parent.childCount < sacrificer.GetComponent<Sacrifice>().sacCount) return false;


		return true; 
	}


	void OnTriggerStay(Collider other){
		if (other.name == "PedestalZone"){
			if ((currentWaypoint == wayParent.childCount - 1) && !advancing){

				pathfinderReady = false;

				//only allow a new one if one has not been created already
				int sacCount = sacrificer.GetComponent<Sacrifice>().sacCount;
				int newVicCount = transform.parent.GetChild(0).gameObject.GetComponent<Pathfinder>().myCount;
				//if we have the right number
				Debug.Log("newVicCount = " + newVicCount);
				if (newVicCount - howMany == sacCount
					&& myCount - 1 == sacCount) 
						pathfinderReady = true;
				

				//Debug.Log("sac ready");
				if (pathfinderReady) sacrificer.GetComponent<Sacrifice>().sacReady = true;
				//imReady = true;
			}
		}

	}

}
