using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	public GameObject bloodJarPrefab;
	public GameObject autosacPrefab;
	public Camera bloodCamera;
	public float bloodJarAmt = 13f;
	public float jarEfficiency = 0.3f;
	public int itemLimit = 7;
	public int bloodJarNumber = 0;
	public int autosacNumber = 0;
	public Transform bloodSpawn;
	public Transform autosacSpawn;
	GameObject diffManager;
	public AudioSource audsrc;
	public AudioClip pourSnd;
	public AudioClip shatterSnd;
	public AudioClip timerSnd;
	public AudioClip campaignSnd;
	public bool failed = false;
	public GameObject[] storeItems = new GameObject[3];
	public int[] storeCosts = new int[3];
	public int storeEntryMin = 100;
	public Transform vicParent;
	public GameObject storeParent;
	public GameObject inventoryParent;


	// Use this for initialization
	void Start () {
		diffManager = GameObject.Find("DifficultyManager");
		failed = false;
		if (inventoryParent == null) inventoryParent = GameObject.Find("InventoryItems");
	}
	
	// Update is called once per frame
	void Update () {
		if (GameState.state == 1 || DifficultyManager.currentDifficulty == 1) {
			if (storeParent.activeSelf == false) storeParent.SetActive(true);
			if (inventoryParent.activeSelf == false) inventoryParent.SetActive(true);
		} else {
			storeParent.SetActive(false);
			if (GameState.state == 3) inventoryParent.SetActive(false);
		}
		itemCast();
		if (failed) GetComponent<Sacrifice>().Fail(3f, "" +	"\"Ungrateful parasites\""); //make fail stuff happen

		//show them after the player has earned enough
		if (!storeItems[storeItems.Length - 1].activeSelf){
			for (int i = 0; i < storeItems.Length; i++){
				if ((GetComponent<Sacrifice>().scoreCount > storeCosts[i] + storeEntryMin)){
					storeItems[i].SetActive(true);
				}
			}

			//if (bloodJarNumber > 0) storeItems[0].SetActive(true);
			//if (autosacNumber > 0) storeItems[1].SetActive(true);
		}
	}



	void itemCast(){
		Ray beam = bloodCamera.ScreenPointToRay(Input.mousePosition);

		//declare and initialize our raycasthit to store hit information

		Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.blue);
		RaycastHit beamHit = new RaycastHit();

		if (Input.GetMouseButtonDown(0)){
			if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("3D-UI"))){

				if (beamHit.collider.gameObject.tag == "jar"){
					useJar(beamHit.collider.gameObject);

				} else if (beamHit.collider.gameObject.tag == "store"){
					purchaseItem(beamHit.collider.transform.parent.gameObject);

				} 


			}
		}
		if (Input.GetButtonDown("Inventory")){
			if (bloodJarNumber > 0){	
				useJar(bloodSpawn.GetChild(bloodSpawn.childCount - 1).gameObject);
			}
		}
	}

	public void purchaseItem(GameObject item){
		//Debug.Log("puchased...");
		int cost = 0;
		for (int i = 0; i < storeItems.Length; i++){
			if (item == storeItems[i]) {
				cost = storeCosts[i];
				if (GetComponent<Sacrifice>().scoreCount > cost){
					GetComponent<Sacrifice>().expenses += cost;
					createItem(i, true);
				} else {
					failed = true;
				}
			}
		}

	}

	void createItem(int itemId, bool purchased){
			switch(itemId){
			case 0:
				createJar(purchased);
				break;
			case 1:
				createAuto();
				break;
			case 2:
				createCampaign();
				break;
			}
	}
	public void createCampaign(){
		audsrc.Stop();
		audsrc.clip = campaignSnd;
		audsrc.Play();

		for (int i = 0; i < vicParent.childCount; i++){
			
			//vicParent.GetChild(i).gameObject.GetComponent<Mood>().mood = 1f;
			vicParent.GetChild(i).GetComponent<Mood>().moodDir = 1f;
			vicParent.GetChild(i).GetComponent<Mood>().moodSpeedMult += 2f;
			vicParent.GetChild(i).GetComponent<Mood>().moodSpeedMult *= 5f;
		}
	}

	public void createJar(bool purchased){
		if (bloodJarNumber < itemLimit){
			bloodJarNumber += 1;

			if (!purchased) GetComponent<BloodMeter>().bloodAmt -= bloodJarAmt;
			GameObject newJar = Instantiate(bloodJarPrefab, bloodSpawn);
			audsrc.Stop();
			audsrc.clip = pourSnd;
			audsrc.Play();
			Debug.Log("playing jar audio " + audsrc.isPlaying);
			Vector3 bldSpwn = bloodSpawn.position;
			bldSpwn += (newJar.transform.up * -1.2f * (bloodJarNumber - 1));
			newJar.transform.position = bldSpwn;
		} else {
			Debug.Log("blood overflow autosac being created...");
			//create auto clicker?
			if (autosacNumber < itemLimit && !GetComponent<Sacrifice>().easyMode) createAuto();

		}

	}


	public void createAuto(){
		//bloodAmt -= (bloodJarAmt * jarEfficiency);

		GetComponent<BloodMeter>().bloodAmt = 5f;

		if (autosacNumber < itemLimit){
			audsrc.Stop();
			//audsrc.PlayOneShot(timerSnd);
			audsrc.clip = timerSnd;
			audsrc.Play();
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
		} else {
			createCampaign();
		}

	}

	public void useJar(GameObject jar){
		GetComponent<BloodMeter>().bloodAmt += (bloodJarAmt * jarEfficiency);
		bloodJarNumber -= 1;
		audsrc.Stop();
		audsrc.clip = shatterSnd;
		audsrc.Play();
		//audsrc.PlayOneShot(shatterSnd);
		Destroy(jar);
	}
}
