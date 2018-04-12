using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips : MonoBehaviour
{
	[TextArea]
	public string[] bloodTips;
	[TextArea]
	public string[] brokerTips;
	[TextArea]
	public string[] clawTips;
	[SerializeField]int currentBloodTip;
	[SerializeField]int currentBrokerTip;
	[SerializeField]int currentClawTip;
	//bool dispBloodTip, dispBrokerTip, dispClawTip;
	public GameObject bloodTipObj;
	public GameObject brokerTipObj;
	public GameObject clawTipObj;
	public GameObject tipPanel;
	public GameObject helpToggle;
	public GameObject forwardButton, backwardButton;
	public Font titleFont;
	public Font tipFont;

	public static bool displayingTip = true;
	public static bool tipsOn = true;
	int pState;
	public Color returnGameColor;

	// Use this for initialization
	void Start ()
	{
		currentClawTip = 0;
		currentBrokerTip = 0;
		currentBloodTip = 0;
		if (bloodTipObj == null)
			bloodTipObj = GameObject.Find ("BloodTip");
		if (brokerTipObj == null)
			brokerTipObj = GameObject.Find ("BrokerTip");
		if (clawTipObj == null)
			clawTipObj = GameObject.Find ("ClawTip");
		if (tipPanel == null)
			tipPanel = GameObject.Find ("TipPanel");

		bloodTipObj.GetComponent<Text> ().text = bloodTips [currentBloodTip];
		brokerTipObj.GetComponent<Text> ().text = brokerTips [currentBrokerTip];
		clawTipObj.GetComponent<Text> ().text = clawTips [currentClawTip];
		//dispBloodTip = true;
		//dispClawTip = true;
		//dispBrokerTip = true;
		if (PlayerPrefs.GetInt("help") == 1){
			helpToggle.GetComponent<Toggle> ().isOn = true;
		} else if (PlayerPrefs.GetInt("help") == 0){
			helpToggle.GetComponent<Toggle> ().isOn = false;
		}
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

		if (currentBloodTip == bloodTips.Length && currentBrokerTip == brokerTips.Length && currentClawTip == clawTips.Length) {
			SetHelpBool (false);
		}

		switch (GameState.state) {
		case -1:
			tipPanel.SetActive (false);
			break;
		case 1:
			if (backwardButton.activeSelf && currentBloodTip == 0){
				backwardButton.SetActive (false);
			}
			if (currentBloodTip != bloodTips.Length) {
				if (currentBloodTip == 0) {
					bloodTipObj.GetComponent<Text> ().font = titleFont;
					bloodTipObj.GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
				} else {
					bloodTipObj.GetComponent<Text> ().font = tipFont;
					bloodTipObj.GetComponent<Text> ().alignment = TextAnchor.MiddleLeft;
				}
				if (currentBloodTip == bloodTips.Length - 1) {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = returnGameColor; 
				} else {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = Color.white;
				}
				bloodTipObj.SetActive (true); 
				if (tipsOn) {
					tipPanel.SetActive (true);
				}
			} else {
				tipPanel.SetActive (false);
			}
			brokerTipObj.SetActive (false);
			clawTipObj.SetActive (false);

			break;
		case 2:
			if (backwardButton.activeSelf && currentBrokerTip == 0)
				backwardButton.SetActive (false);
			if (currentBrokerTip != brokerTips.Length) {
				if (currentBrokerTip == 0) {
					brokerTipObj.GetComponent<Text> ().font = titleFont;
					brokerTipObj.GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
				} else {
					brokerTipObj.GetComponent<Text> ().font = tipFont;
					brokerTipObj.GetComponent<Text> ().alignment = TextAnchor.MiddleLeft;
				}
				if (currentBrokerTip == brokerTips.Length - 1) {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = returnGameColor; 
				} else {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = Color.white;
				}
				brokerTipObj.SetActive (true); 
				if (Mathf.Approximately (CameraMove.zoom, 1f) && tipsOn) {
					tipPanel.SetActive (true);
				}
			} else { 
				tipPanel.SetActive (false);
			}
			bloodTipObj.SetActive (false);
			clawTipObj.SetActive (false);
			
			break;
		case 3:
			if (backwardButton.activeSelf && currentClawTip == 0)
				backwardButton.SetActive (false);
			if (currentClawTip != clawTips.Length) {
				if (currentClawTip == 0) {
					clawTipObj.GetComponent<Text> ().font = titleFont;
					clawTipObj.GetComponent<Text> ().alignment = TextAnchor.MiddleCenter;
				} else {
					clawTipObj.GetComponent<Text> ().font = tipFont;
					clawTipObj.GetComponent<Text> ().alignment = TextAnchor.MiddleLeft;
				}
				if (currentClawTip == clawTips.Length - 1) {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = returnGameColor; 
				} else {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = Color.white;
				}
				clawTipObj.SetActive (true); 
				if (Mathf.Approximately (CameraMove.zoom, 1f) && tipsOn) {
					tipPanel.SetActive (true);
				}
			} else {
				tipPanel.SetActive (false);
			}
			brokerTipObj.SetActive (false);
			bloodTipObj.SetActive (false);

			break;

		}

		pState = GameState.state;
			
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

		bloodTipObj.GetComponent<Text> ().text = bloodTips [currentBloodTip];
		brokerTipObj.GetComponent<Text> ().text = brokerTips [currentBrokerTip];
		clawTipObj.GetComponent<Text> ().text = clawTips [currentClawTip];
	}

	public void AdvanceTip ()
	{
		switch (GameState.state) {
		case 1:
			currentBloodTip++;
			if (currentBloodTip < bloodTips.Length) {	
				if (!backwardButton.activeSelf)
					backwardButton.SetActive (true);	
				bloodTipObj.GetComponent<Text> ().text = bloodTips [currentBloodTip];
				//if (currentBloodTip == bloodTips.Length - 1) currentBloodTip++; //overflow so wont display again
			} else {
				tipPanel.SetActive (false);
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
			if (currentBloodTip > 0) {	

				currentBloodTip--;
				bloodTipObj.GetComponent<Text> ().text = bloodTips [currentBloodTip];
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
