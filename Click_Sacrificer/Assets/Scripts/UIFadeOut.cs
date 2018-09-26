using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeOut : MonoBehaviour {

	Text myText;
	public Color startColor, endColor;
	public float speed = 1f;
	// Use this for initialization
	void Start () {
		myText = GetComponent<Text>();

	}
	
	// Update is called once per frame
	void Update () {
		myText.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));

	}
}