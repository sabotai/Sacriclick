using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodMeter : MonoBehaviour {

	public float bloodAmt = 100f;
	[SerializeField] float secondsRemaining = 0f;
	public float sacBloodValue = 10f;
	public float bloodSecondRatio = 0.1f;
	public bool failureAllowed = false;
	public RectTransform bloodUI;
	public float restartTimeoutAmount = 3f;
	float bloodUIOrigY;
	bool failed;
	GameObject bloodCanvasItem;

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

			((MovieTexture)bloodCanvasItem.GetComponent<RawImage>().mainTexture).Stop();
		} else {
			((MovieTexture)bloodCanvasItem.GetComponent<RawImage>().mainTexture).Play();
			bloodAmt -= Time.deltaTime * bloodSecondRatio; //1 ratio is 1:1 seconds to blood
			secondsRemaining = bloodAmt / bloodSecondRatio;
		}


		if (bloodAmt < 0.01f && failureAllowed) failed = true; //start fail action frames
		if (failed)	GetComponent<Sacrifice>().Fail(restartTimeoutAmount); //make fail stuff happen
		
		bloodAmt = Mathf.Clamp(bloodAmt, 0f, 25f); //dont allow to go below zero or over 30 for ui purposes
		bloodUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bloodAmt); //sets the blood movement on screen
	}

}
