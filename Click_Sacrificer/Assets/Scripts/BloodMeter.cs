using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BloodMeter : MonoBehaviour {

	public float bloodAmt = 100f;
	[SerializeField] float secondsRemaining = 0f;
	public float sacBloodValue = 10f;
	float origSacBloodValue;
	public float bloodSecondRatio = 0.1f;
	public bool failureAllowed = false;
	public RectTransform bloodUI;
	public Material bloodMat;
	public float restartTimeoutAmt = 3f;
	public float bloodJarAmt = 15f;
	public float jarEfficiency = 0.3f;
	public float bloodScreenAmt = 20f;
	public int jarLimit = 8;
	int bloodJarNumber = 0;
	public int autosacNumber = 0;
	float bloodUIOrigY;
	public bool failed;
	GameObject bloodCanvasItem;
	public VideoPlayer bloodPlayer;
	public GameObject bloodJarPrefab;
	public GameObject autosacPrefab;
	public Camera bloodCamera;
	public Transform bloodSpawn;
	public Transform autosacSpawn;
	GameObject diffManager;
	public AudioClip pourSnd;
	public AudioClip shatterSnd;
	public AudioClip timerSnd;
	public AudioSource audsrc;
	public bool useMood = true;
	GameObject victims;
	public bool useAutoJar = true;
	bool firstClick = false;

	// Use this for initialization
	void Start () {
		diffManager = GameObject.Find("DifficultyManager");
		secondsRemaining = bloodAmt * bloodSecondRatio;
		bloodUIOrigY = bloodUI.localPosition.y;
		failed = false;
		bloodCanvasItem = GameObject.Find("RawImage");
		victims = GameObject.Find("Victims");
		origSacBloodValue = sacBloodValue;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameState.state == -1 || Tips.displayingTip) bloodPlayer.Stop();
		if (!Tips.displayingTip && (GameState.state == 1 || GameState.state == 2)){
			if (bloodAmt < 0.01f && failureAllowed) failed = true; //start fail action frames
			if (failed){
				GetComponent<Sacrifice>().Fail(restartTimeoutAmt, "The gods are displeased."); //make fail stuff happen
			} else {
				if (!firstClick && GetComponent<Sacrifice>().sacCount > 0){
					firstClick = true;
				}
				if (GetComponent<Sacrifice>().easyMode || GameState.state == -1){ //use easy mode as a debug to stop blood
					failureAllowed = false;

					bloodPlayer.Stop();
					//((MovieTexture)bloodCanvasItem.GetComponent<RawImage>().mainTexture).Stop();
				} else {
					//((MovieTexture)bloodCanvasItem.GetComponent<RawImage>().mainTexture).Play();
					bloodPlayer.Play();
					failureAllowed = true;
					if (firstClick) bloodAmt -= Time.deltaTime * bloodSecondRatio; //1 ratio is 1:1 seconds to blood
					secondsRemaining = bloodAmt / bloodSecondRatio;
				}
				if (bloodAmt > bloodScreenAmt){ //increment blood jars if enough blood is shed
					if (bloodJarNumber < jarLimit){
						createJar();
					} else {
						Debug.Log("blood overflow autosac being created...");
						//create auto clicker?
						if (autosacNumber < jarLimit) createAuto();

					}

				}

				if (bloodAmt < 0.01f && bloodJarNumber > 0 && useAutoJar){
					useJar(bloodSpawn.GetChild(bloodSpawn.childCount - 1).gameObject);
				}

				bloodAmt = Mathf.Clamp(bloodAmt, 0f, 20f); //dont allow to go below zero or over 30 for ui purposes
				Color bloodColor;
				bloodColor = bloodMat.GetColor("_TintColor");
				float bloodPct = bloodAmt / 20f;
				float maxA = 0.22f;
				float bloodA = maxA - maxA * (bloodAmt / 20f);
				bloodMat.SetColor("_TintColor", new Color (bloodColor.r, bloodColor.g, bloodColor.b, bloodA));
				bloodUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bloodAmt); //sets the blood movement on screen

				jarCast();
			}
		}
	}

	public void updateMood(){		
		if (useMood) {	
			//	old take from back method
			//	int leader = victims.transform.childCount - 1 - GetComponent<Sacrifice>().sacCount;
			int leader = 0 - GetComponent<Sacrifice>().sacCount;
			if (leader < 0) leader = 0;
			sacBloodValue = origSacBloodValue * victims.transform.GetChild(leader).gameObject.GetComponent<Mood>().mood;
			//Debug.Log("currentSacBloodValue = " + sacBloodValue);
		}
	}
	void useJar(GameObject jar){
		bloodAmt += (bloodJarAmt * jarEfficiency);
		bloodJarNumber -= 1;
		audsrc.PlayOneShot(shatterSnd);
		Destroy(jar);
	}
	void jarCast(){
		Ray beam = bloodCamera.ScreenPointToRay(Input.mousePosition);

		//declare and initialize our raycasthit to store hit information

		Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.blue);
		RaycastHit beamHit = new RaycastHit();

		if (Input.GetMouseButtonDown(0)){
			if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("3D-UI"))){

				if (beamHit.collider.gameObject.tag == "jar"){
					useJar(beamHit.collider.gameObject);
				}

			}
		}
		if (Input.GetButtonDown("Inventory")){
				if (bloodJarNumber > 0){	
					useJar(bloodSpawn.GetChild(bloodSpawn.childCount - 1).gameObject);
				}
		}
	}

	void createJar(){

		bloodJarNumber += 1;
		bloodAmt -= bloodJarAmt;
		//GameObject newJar = Instantiate(bloodJar, bloodSpawn.position, Quaternion.identity);
		GameObject newJar = Instantiate(bloodJarPrefab, bloodSpawn);
		audsrc.PlayOneShot(pourSnd);
		Vector3 bldSpwn = bloodSpawn.position;
		//bldSpwn += new Vector3(0,-1.2f,0f) * (bloodJarNumber - 1);
		bldSpwn += (newJar.transform.up * -1.2f * (bloodJarNumber - 1));
		//Debug.Log(bldSpwn);
		newJar.transform.position = bldSpwn;
		//newJar.transform.position = bloodSpawn.position;
		//newJar.transform.parent = bloodSpawn;

	}
	public void createAuto(){
		//bloodAmt -= (bloodJarAmt * jarEfficiency);
		bloodAmt = 5f;

		audsrc.PlayOneShot(timerSnd);
		autosacNumber++;
		//bloodAmt -= (bloodJarAmt * 0.8f);
		diffManager.GetComponent<Autosac>().numAutosacs = autosacNumber;
		diffManager.GetComponent<Autosac>().clicksRemaining = autosacNumber * diffManager.GetComponent<Autosac>().numClicks;
		GameObject newAutosac = Instantiate(autosacPrefab, autosacSpawn);
		//audsrc.PlayOneShot(pourSnd);
		Vector3 autoSpwn = autosacSpawn.position;
		//bldSpwn += new Vector3(0,-1.2f,0f) * (bloodJarNumber - 1);
		autoSpwn += (-newAutosac.transform.right * -1.2f * (autosacNumber - 1));
		newAutosac.transform.position = autoSpwn;
		//Debug.Log("spawn auto... " + );

	}

}
