﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips : MonoBehaviour
{
	public int numColumns = 10;
	public int bloodTipNum = 0;
	public int storeTipNum = 0;
	public int brokerTipNum = 0;
	public int clawTipNum = 0;

	[TextArea]
	public string[] bloodTips;
	[TextArea]
	public string[] storeTips;
	[TextArea]
	public string[] brokerTips;
	[TextArea]
	public string[] clawTips;
	[SerializeField]int currentBloodTip;
	[SerializeField]int currentBrokerTip;
	[SerializeField]int currentClawTip;
	[SerializeField]int currentStoreTip;
	//bool dispBloodTip, dispBrokerTip, dispClawTip;
	public GameObject bloodTipObj;
	public GameObject storeTipObj;
	public GameObject brokerTipObj;
	public GameObject clawTipObj;
	public GameObject tipPanel;
	public GameObject helpToggle;
	public GameObject forwardButton, backwardButton;
	public Font titleFont;
	public Font tipFont;
	int storeMinimum = 0;

	public static bool displayingTip = true;
	public static bool tipsOn = true;
	int pState;
	public Color returnGameColor;

	public string language = "English";
	public TextAsset csvFile; // Reference of CSV file
	//public InputField rollNoInputField;// Reference of rollno input field
	//public InputField nameInputField; // Reference of name input filed
	//public Text contentArea; // Reference of contentArea where records are displayed
	 
	private char lineSeperater = '\n'; // It defines line seperate character
	private char fieldSeperator = ';'; // It defines field seperate chracter
	public string[,] data;

	// Use this for initialization
	void Start ()
	{
		bloodTips = new string[numColumns];
		storeTips = new string[numColumns];
		brokerTips = new string[numColumns];
		clawTips = new string[numColumns];
		readData();

		currentClawTip = 0;
		currentBrokerTip = 0;
		currentBloodTip = 0;
		currentStoreTip = 0;
		if (bloodTipObj == null)
			bloodTipObj = GameObject.Find ("BloodTip");
		if (brokerTipObj == null)
			brokerTipObj = GameObject.Find ("BrokerTip");
		if (clawTipObj == null)
			clawTipObj = GameObject.Find ("ClawTip");
		if (storeTipObj == null)
			clawTipObj = GameObject.Find ("StoreTip");
		if (tipPanel == null)
			tipPanel = GameObject.Find ("TipPanel");

     
/*
		bloodTipObj.GetComponent<Text> ().text = bloodTips [currentBloodTip];
		brokerTipObj.GetComponent<Text> ().text = brokerTips [currentBrokerTip];
		clawTipObj.GetComponent<Text> ().text = clawTips [currentClawTip];
		storeTipObj.GetComponent<Text> ().text = storeTips [currentStoreTip];
*/
		//dispBloodTip = true;
		//dispClawTip = true;
		//dispBrokerTip = true;
		if (PlayerPrefs.GetInt("help") == 1){
			helpToggle.GetComponent<Toggle> ().isOn = true;
		} else if (PlayerPrefs.GetInt("help") == 0){
			helpToggle.GetComponent<Toggle> ().isOn = false;
		}
		storeMinimum = Camera.main.GetComponent<Inventory>().storeCosts[0] + Camera.main.GetComponent<Inventory>().storeEntryMin;
	}

	private void readData()	{
		string[] lines = csvFile.text.Split (lineSeperater);
		Debug.Log("# lines = " + lines.Length);

		data = new string[lines.Length,10];
		for (int i = 0; i < lines.Length; i++){
			string[] fields = lines[i].Split(fieldSeperator);
			for (int j = 0; j < fields.Length-2; j++){

				data[i,j] = fields[j]; //storing all our lines and fields in data

				if (fields[0] == language){
					if (fields[1] == "HowTo_Blood"){
						bloodTips[j] = fields[j+2];
						Debug.Log("field =" + bloodTips[j] + "; length =" + bloodTips[j].Length + ";");
						if (bloodTips[j].Length <= 1 && bloodTipNum == 0) {
							bloodTipNum = j;
							Debug.Log("bloodtipnum = " + bloodTipNum);
						}
					}
					if (fields[1] == "HowTo_Broker"){
						brokerTips[j] = fields[j+2];
						if (brokerTips[j].Length <= 1 && brokerTipNum == 0) {
							brokerTipNum = j;
						}
					}
					if (fields[1] == "HowTo_Store"){
						storeTips[j] = fields[j+2];
						if (storeTips[j].Length <= 1 && storeTipNum == 0) {
							storeTipNum = j;
						}
					}
					if (fields[1] == "HowTo_Minigame"){
						clawTips[j] = fields[j+2];
						if (clawTips[j].Length <= 1 && clawTipNum == 0) {
							clawTipNum = j;
						}
					}
				}
				//contentArea.text += field + "\t";
			}
		}

		cleanUp();
	}

	void cleanUp(){
		string[] newBloodTips = new string[bloodTipNum];
		System.Array.Copy(bloodTips, newBloodTips, bloodTipNum); 
		bloodTips = newBloodTips;

		string[] newBrokerTips = new string[brokerTipNum];
		System.Array.Copy(brokerTips, newBrokerTips, brokerTipNum); 
		brokerTips = newBrokerTips;

		string[] newStoreTips = new string[storeTipNum];
		System.Array.Copy(storeTips, newStoreTips, storeTipNum); 
		storeTips = newStoreTips;

		string[] newClawTips = new string[clawTipNum];
		System.Array.Copy(clawTips, newClawTips, clawTipNum); 
		clawTips = newClawTips;
	}

	// Update is called once per frame
	void Update ()
	{

		if (PlayerPrefs.GetInt ("help") == 0)
			tipPanel.SetActive (false);

		if (tipPanel.activeSelf){
			displayingTip = true;
		}
		else{
			displayingTip = false;
		}

		if (currentBloodTip == bloodTips.Length && currentBrokerTip == brokerTips.Length && currentClawTip == clawTips.Length && currentStoreTip == storeTips.Length) {
			SetHelpBool (false);
		}

		switch (GameState.state) {
		case -1:
			tipPanel.SetActive (false);
			break;
		case 1:			
			if (Camera.main.GetComponent<Sacrifice>().scoreCount <= storeMinimum){
				DisplayTips(bloodTipObj, currentBloodTip, bloodTips);
				brokerTipObj.SetActive (false);
				clawTipObj.SetActive (false);
				storeTipObj.SetActive (false);
			} else {
				if (currentStoreTip == 0 && PlayerPrefs.GetInt ("help") == 1)
					tipPanel.SetActive (true);
				DisplayTips(storeTipObj, currentStoreTip, storeTips);
				brokerTipObj.SetActive (false);
				clawTipObj.SetActive (false);
				bloodTipObj.SetActive (false);
			}
	

			break;
		case 2:
			DisplayTips(brokerTipObj, currentBrokerTip, brokerTips);
			bloodTipObj.SetActive (false);
			clawTipObj.SetActive (false);
			storeTipObj.SetActive (false);
			
			break;
		case 3:
			DisplayTips(clawTipObj, currentClawTip, clawTips);
			brokerTipObj.SetActive (false);
			bloodTipObj.SetActive (false);
			storeTipObj.SetActive (false);

			break;

		}

		pState = GameState.state;
			
	}

	public void DisplayTips(GameObject currentTipObj, int currentTip, string[] tips){
			if (backwardButton.activeSelf && currentTip == 0)
				backwardButton.SetActive (false);
			if (currentTip != tips.Length) {
				if (currentTip == 0) {
					currentTipObj.GetComponent<Text> ().font = titleFont;
					currentTipObj.GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
				} else {
					currentTipObj.GetComponent<Text> ().font = tipFont;
					currentTipObj.GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
				}
				if (currentTip == tips.Length - 1) {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = returnGameColor; 
				} else {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = Color.white;
				}
				currentTipObj.SetActive (true); 
				if (Mathf.Approximately (CameraMove.zoom, 1f) && tipsOn) {
					tipPanel.SetActive (true);
				}
			} else {
				tipPanel.SetActive (false);
			}

	}

	public void SetHelpBool (bool helpOn)
	{
		tipsOn = helpOn;

		int helpInt = 0;
		if (tipsOn) {
			helpInt = 1;			
		} 
		PlayerPrefs.SetInt ("help", helpInt);
		helpToggle.GetComponent<Toggle> ().isOn = tipsOn;
		currentBloodTip = 0;
		currentBrokerTip = 0;
		currentClawTip = 0;
		currentStoreTip = 0;

		bloodTipObj.GetComponent<Text> ().text = bloodTips [currentBloodTip];
		brokerTipObj.GetComponent<Text> ().text = brokerTips [currentBrokerTip];
		clawTipObj.GetComponent<Text> ().text = clawTips [currentClawTip];
		storeTipObj.GetComponent<Text> ().text = storeTips [currentStoreTip];
	}

	public void AdvanceTip ()
	{
		switch (GameState.state) {
		case 1:
			if (Camera.main.GetComponent<Sacrifice>().scoreCount < storeMinimum){
				currentBloodTip++;
				if (currentBloodTip < bloodTips.Length) {	
					if (!backwardButton.activeSelf)
						backwardButton.SetActive (true);	
					bloodTipObj.GetComponent<Text> ().text = bloodTips [currentBloodTip];
					//if (currentBloodTip == bloodTips.Length - 1) currentBloodTip++; //overflow so wont display again
				} else {
					tipPanel.SetActive (false);
				}
			} else {

				currentStoreTip++;
				if (currentStoreTip < storeTips.Length) {	
					if (!backwardButton.activeSelf)
						backwardButton.SetActive (true);	
					storeTipObj.GetComponent<Text> ().text = storeTips [currentStoreTip];
					//if (currentBloodTip == bloodTips.Length - 1) currentBloodTip++; //overflow so wont display again
				} else {
					tipPanel.SetActive (false);
				}
			}
			break;
			
		case 2:
			currentBrokerTip++;
			if (currentBrokerTip < brokerTips.Length) {
				if (!backwardButton.activeSelf)
					backwardButton.SetActive (true);	
				brokerTipObj.GetComponent<Text> ().text = brokerTips [currentBrokerTip];
				//if (currentBrokerTip == brokerTips.Length - 1) currentBrokerTip++;
			} else {
				tipPanel.SetActive (false);
			}
			break;
		case 3:
			currentClawTip++;
			if (currentClawTip < clawTips.Length) {	
				if (!backwardButton.activeSelf)
					backwardButton.SetActive (true);	
				clawTipObj.GetComponent<Text> ().text = clawTips [currentClawTip];
				//if (currentClawTip == clawTips.Length - 1) currentClawTip++;
			} else {
				tipPanel.SetActive (false);
			}
			break;
		}

	}

	public void PreviousTip ()
	{
		switch (GameState.state) {
		case 1:
			if (Camera.main.GetComponent<Sacrifice>().scoreCount < storeMinimum){

				currentBloodTip--;
				bloodTipObj.GetComponent<Text> ().text = bloodTips [currentBloodTip];
			} else {

				currentStoreTip--;
				storeTipObj.GetComponent<Text> ().text = storeTips [currentStoreTip];
			}

			break;
		case 2:
			if (currentBrokerTip > 0) {	
				currentBrokerTip--;
				brokerTipObj.GetComponent<Text> ().text = brokerTips [currentBrokerTip];
			}
			break;
		case 3:
			if (currentClawTip > 0) {	
				currentClawTip--;
				clawTipObj.GetComponent<Text> ().text = clawTips [currentClawTip];
			} 
			break;
		}


	}
}