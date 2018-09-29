using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodNotification : MonoBehaviour {

	float amt = 0f;
	public float limit = 1f;
	public string text;
	public float moodMod;
	bool begin = false;
	public float speed = 1f;
	float origLimit;
	Vector3 origPos;
	Vector3 origScale;
	// Use this for initialization
	void Start () {
		origLimit = limit;
		origPos = transform.position;
		origScale = transform.localScale;
	}
	void OnEnable(){
		begin = true;
		transform.localScale *= Mathf.Clamp(Mathf.Abs(moodMod), 0.1f, 0.25f);

		//move up for larger ones to make them clearer
		transform.Translate(new Vector3(0.7f,1f,0f) * Mathf.Abs(moodMod), Space.World);
		float pctMood = (int)(moodMod * 100f);
		Color setColor;
		if (pctMood > 0f) {
			text = "+" + pctMood.ToString() + "%";
			if (ColorblindMode.cbMode) setColor = ColorblindMode.cbGreen;
			else setColor = Color.green;
		} else {
			text = pctMood.ToString() + "%";		
			GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, Mathf.Abs(pctMood)/100f);

			if (ColorblindMode.cbMode) setColor = ColorblindMode.cbRed;
			else setColor = Color.red;
		}

		GetComponent<MeshRenderer>().material.color = Color.Lerp(setColor, Color.white, 0.7f); //use a slightly different color to make it easier to see

		GetComponent<TextMesh>().text = text;


	}
	void reset(){
		GetComponent<MeshRenderer>().material.color = new Color(0f,0f,0f,0f);
		transform.position = origPos; //reset pos
      	limit = origLimit; //reset limit
      	text = ""; //reset text
      	gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		if (begin){

			amt = Time.deltaTime * speed;
			limit -= amt;
			float pct = limit / origLimit;

			GetComponent<MeshRenderer>().material.color = Color.Lerp(new Color(0f,0f,0f,0f), GetComponent<MeshRenderer>().material.color, pct);
      		transform.Translate(Vector3.up * amt, Space.World);
      		if (limit < 0f) {
      			begin = false;
      			reset();
      		}


		}
	}
}
