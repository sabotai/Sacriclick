using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour {
	public static string testStaticString;
	public string testString;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void renameString(string myS){
		testString = myS;
	}
}
