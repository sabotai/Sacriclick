using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BloodMeter : MonoBehaviour {

	public float bloodAmt = 100f;
	[SerializeField] float secondsRemaining = 0f;
	public float sacBloodValue = 10f;
	public float bloodSecondRatio = 0.1f;
	public bool failureAllowed = false;
	public RectTransform bloodUI;
	public float restartTimeoutAmt = 3f;
	public float bloodJarAmt = 15f;
	public float jarEfficiency = 0.3f;
	public float bloodScreenAmt = 20f;
	int bloodJarNumber = 0;
	float bloodUIOrigY;
	bool failed;
	GameObject bloodCanvasItem;
	public VideoPlayer bloodPlayer;
	public GameObject bloodJar;
	public Camera bloodCamera;
	public Transform bloodSpawn;
	public AudioClip pourSnd;
	public AudioClip shatterSnd;
	public AudioSource audsrc;

	// Use this for initialization
	void Start () {
		secondsRemaining = bloodAmt * bloodSecondRatio;
		bloodUIOrigY = bloodUI.localPosition.y;
		failed = false;
		bloodCanvasItem = GameObject.Find("RawImage");
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Sacrifice>().easyMode){ //use easy mode as a debug to stop blood
			failureAllowed = false;

			bloodPlayer.Stop();
			//((MovieTexture)bloodCanvasItem.GetComponent<RawImage>().mainTexture).Stop();
		} else {
			//((MovieTexture)bloodCanvasItem.GetComponent<RawImage>().mainTexture).Play();
			bloodPlayer.Play();
			bloodAmt -= Time.deltaTime * bloodSecondRatio; //1 ratio is 1:1 seconds to blood
			secondsRemaining = bloodAmt / bloodSecondRatio;
		}


		if (bloodAmt < 0.01f && failureAllowed) failed = true; //start fail action frames
		if (failed)	GetComponent<Sacrifice>().Fail(restartTimeoutAmt); //make fail stuff happen
		
		if (bloodAmt > bloodScreenAmt){ //increment blood jars if enough blood is shed
			if (bloodJarNumber < 8){
				bloodJarNumber += 1;
				bloodAmt -= bloodJarAmt;
				//GameObject newJar = Instantiate(bloodJar, bloodSpawn.position, Quaternion.identity);
				GameObject newJar = Instantiate(bloodJar, bloodSpawn);
				audsrc.PlayOneShot(pourSnd);
				Vector3 bldSpwn = bloodSpawn.position;
				//bldSpwn += new Vector3(0,-1.2f,0f) * (bloodJarNumber - 1);
				bldSpwn += (newJar.transform.up * -1.2f * (bloodJarNumber - 1));
				//Debug.Log(bldSpwn);
				newJar.transform.position = bldSpwn;
				//newJar.transform.position = bloodSpawn.position;
				//newJar.transform.parent = bloodSpawn;
			}

		}
		bloodAmt = Mathf.Clamp(bloodAmt, 0f, 20f); //dont allow to go below zero or over 30 for ui purposes
		bloodUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bloodAmt); //sets the blood movement on screen

		jarCast();
	}

	void jarCast(){
		Ray beam = bloodCamera.ScreenPointToRay(Input.mousePosition);

		//declare and initialize our raycasthit to store hit information

		Debug.DrawRay(beam.origin, beam.direction * 1000f, Color.blue);
		RaycastHit beamHit = new RaycastHit();

		if (Input.GetMouseButtonDown(0)){
			if (Physics.Raycast(beam, out beamHit, 1000f, LayerMask.GetMask("3D-UI"))){

				if (beamHit.collider.gameObject.tag == "jar"){
					bloodAmt += (bloodJarAmt * jarEfficiency);
					bloodJarNumber -= 1;
					audsrc.PlayOneShot(shatterSnd);
					Destroy(beamHit.collider.gameObject);
				}

			}
		}
	}

}
