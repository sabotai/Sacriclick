using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsentMeter : MonoBehaviour {
	Color consentColor;
	public float consentPct;
	public float decreaseAmt;
	GameObject mommy;

	// Use this for initialization
	void Start () {
		mommy = transform.parent.gameObject;
		consentPct = mommy.GetComponent<Mood>().mood;
		consentPct += 1f;
		consentPct /= 2f; //convert back to %
	}
	
	// Update is called once per frame
	void Update () {
		if (consentPct > 0.5f){
			consentColor = Color.Lerp(Color.yellow, Color.green, consentPct - 0.5f);
		} else {
			consentColor = Color.Lerp(Color.red, Color.yellow, consentPct + 0.5f);			
		}
		GetComponent<TextMesh>().color = consentColor;
		//consentPct -= decreaseAmt * Time.deltaTime;
		consentPct = mommy.GetComponent<Mood>().mood;
		
	}
}
