using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add the TextMesh Pro namespace to access the various functions.

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
	[SerializeField]public static int currentBloodTip;
	[SerializeField]public static int currentBrokerTip;
	[SerializeField]public static int currentClawTip;
	[SerializeField]public static int currentStoreTip;
	//bool dispBloodTip, dispBrokerTip, dispClawTip;
	public GameObject bloodTipObj;
	public GameObject storeTipObj;
	public GameObject brokerTipObj;
	public GameObject clawTipObj;
	public GameObject tipPanel;
	public Toggle[] helpToggles;
	public GameObject forwardButton, backwardButton;
	public TMP_FontAsset titleFontTMP, tipFontTMP;
	public Font titleFont;
	public Font tipFont;
	int storeMinimum = 0;
	public AudioClip inClip, outClip;

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

		/*currentClawTip = 0;
		currentBrokerTip = 0;
		currentBloodTip = 0;
		currentStoreTip = 0;*/
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

     


		//dispBloodTip = true;
		//dispClawTip = true;
		//dispBrokerTip = true;
		foreach (Toggle helpToggle in helpToggles){
			if (PlayerPrefs.GetInt("help") == 1){
				helpToggle.isOn = true;
			} else if (PlayerPrefs.GetInt("help") == 0){
				helpToggle.isOn = false;
			}
		}
		storeMinimum = Camera.main.GetComponent<Inventory>().storeCosts[0] * Camera.main.GetComponent<Inventory>().storeEntryMin;



		bloodTipObj.GetComponent<TextMeshProUGUI> ().text = bloodTips [currentBloodTip];
		brokerTipObj.GetComponent<TextMeshProUGUI> ().text = brokerTips [currentBrokerTip];
		clawTipObj.GetComponent<TextMeshProUGUI> ().text = clawTips [currentClawTip];
		storeTipObj.GetComponent<TextMeshProUGUI> ().text = storeTips [currentStoreTip];

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
		} else {
			displayingTip = false;
		}
		//if (displayingTip && Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) AdvanceTip ();

		if (currentBloodTip == bloodTips.Length && currentBrokerTip == brokerTips.Length && currentClawTip == clawTips.Length && currentStoreTip == storeTips.Length) {
			SetHelpBool (false);
		}

		switch (GameState.state) {
		case -1:
			tipPanel.SetActive (false);
			break;
		case 1:	
			if (Camera.main.GetComponent<CameraMove>().oldZoom < .85f && Camera.main.GetComponent<CameraMove>().oldZoom > .25f	){	//make sure camera has moved on from intro
				if (PlayerPrefs.GetInt ("help") == 1){

					if (!tipPanel.activeSelf && currentBloodTip == 0)	{

						GetComponent<AudioSource>().PlayOneShot(inClip, 0.75f);
						tipPanel.SetActive (true);
						bloodTipObj.SetActive(true);

					} else {
						if (Camera.main.GetComponent<Sacrifice>().scoreCount <= storeMinimum || currentBloodTip == 0){

							DisplayTips(bloodTipObj, currentBloodTip, bloodTips);
							brokerTipObj.SetActive (false);
							clawTipObj.SetActive (false);
							storeTipObj.SetActive (false);
						} else {
							if (currentBloodTip > 0 && currentStoreTip == 0 && PlayerPrefs.GetInt ("help") == 1 && !tipPanel.activeSelf){
								GetComponent<AudioSource>().PlayOneShot(inClip, .75f);
								tipPanel.SetActive (true);

							}
							DisplayTips(storeTipObj, currentStoreTip, storeTips);
							brokerTipObj.SetActive (false);
							clawTipObj.SetActive (false);
							bloodTipObj.SetActive (false);
						}
					}
				}
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

				

				//update the tips if the fading is done
				if (bloodTipObj.GetComponent<FadeTMP>().fadeIn && currentBloodTip != bloodTips.Length)
					bloodTipObj.GetComponent<TextMeshProUGUI> ().text = bloodTips [currentBloodTip];

				if (brokerTipObj.GetComponent<FadeTMP>().fadeIn && currentBrokerTip != brokerTips.Length)
				brokerTipObj.GetComponent<TextMeshProUGUI> ().text = brokerTips [currentBrokerTip];

				if (clawTipObj.GetComponent<FadeTMP>().fadeIn && currentClawTip != clawTips.Length)
				clawTipObj.GetComponent<TextMeshProUGUI> ().text = clawTips [currentClawTip];

				if (storeTipObj.GetComponent<FadeTMP>().fadeIn && currentStoreTip != storeTips.Length)
				storeTipObj.GetComponent<TextMeshProUGUI> ().text = storeTips [currentStoreTip];

				if (currentTip == tips.Length - 1) {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = returnGameColor; 
				} else {
					forwardButton.transform.GetChild (0).gameObject.GetComponent<Text> ().color = Color.white;
				}
				if (!currentTipObj.activeSelf) {
					GetComponent<AudioSource>().PlayOneShot(inClip, .75f);
					currentTipObj.SetActive (true); 
				}
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
		foreach (Toggle helpToggle in helpToggles){
			helpToggle.isOn = tipsOn;
		}
		if (!helpOn || PlayerPrefs.GetInt ("init") == 0){
			currentBloodTip = 0;
			currentBrokerTip = 0;
			currentClawTip = 0;
			currentStoreTip = 0;
			bloodTipObj.GetComponent<TextMeshProUGUI> ().text = bloodTips [currentBloodTip];
			brokerTipObj.GetComponent<TextMeshProUGUI> ().text = brokerTips [currentBrokerTip];
			clawTipObj.GetComponent<TextMeshProUGUI> ().text = clawTips [currentClawTip];
			storeTipObj.GetComponent<TextMeshProUGUI> ().text = storeTips [currentStoreTip];
		}

	}

	public void AdvanceTip ()
	{
		switch (GameState.state) {
		case 1:
			if (Camera.main.GetComponent<Sacrifice>().scoreCount < storeMinimum){
				currentBloodTip++;
				Debug.Log("currentBloodTip= " + currentBloodTip);
				if (currentBloodTip < bloodTips.Length) {	
					if (!backwardButton.activeSelf)
						backwardButton.SetActive (true);	
					//bloodTipObj.GetComponent<TextMeshProUGUI> ().text = bloodTips [currentBloodTip];
					bloodTipObj.GetComponent<FadeTMP>().FadeOut();
					//if (currentBloodTip == bloodTips.Length - 1) currentBloodTip++; //overflow so wont display again

					GetComponent<AudioSource>().Stop();
					GetComponent<AudioSource>().Play();
				} else {
					GetComponent<AudioSource>().Stop();
					GetComponent<AudioSource>().PlayOneShot(outClip, 0.75f);
					tipPanel.SetActive (false);
				}
			} else {

				currentStoreTip++;
				if (currentStoreTip < storeTips.Length) {	
					if (!backwardButton.activeSelf)
						backwardButton.SetActive (true);	
					storeTipObj.GetComponent<FadeTMP>().FadeOut();
					//if (currentBloodTip == bloodTips.Length - 1) currentBloodTip++; //overflow so wont display again


					GetComponent<AudioSource>().Stop();
					GetComponent<AudioSource>().Play();
				} else {
				GetComponent<AudioSource>().Stop();
					GetComponent<AudioSource>().PlayOneShot(outClip, 0.75f);
					tipPanel.SetActive (false);
				}
			}
			break;
			
		case 2:
			currentBrokerTip++;
			if (currentBrokerTip < brokerTips.Length) {
				if (!backwardButton.activeSelf){
					backwardButton.SetActive (true);
				}

				

				
				brokerTipObj.GetComponent<FadeTMP>().FadeOut();
				
				//if (currentBrokerTip == brokerTips.Length - 1) currentBrokerTip++;

					GetComponent<AudioSource>().Stop();
					GetComponent<AudioSource>().Play();
			} else {
				GetComponent<AudioSource>().Stop();
				GetComponent<AudioSource>().PlayOneShot(outClip, 0.75f);
				tipPanel.SetActive (false);
			}
			break;
		case 3:
			currentClawTip++;
			if (currentClawTip < clawTips.Length) {	
				if (!backwardButton.activeSelf)
					backwardButton.SetActive (true);	
				clawTipObj.GetComponent<FadeTMP>().FadeOut();
				//if (currentClawTip == clawTips.Length - 1) currentClawTip++;


					GetComponent<AudioSource>().Stop();
					GetComponent<AudioSource>().Play();
			} else {
				GetComponent<AudioSource>().Stop();
				GetComponent<AudioSource>().PlayOneShot(outClip, 0.75f);
				tipPanel.SetActive (false);
			}
			break;
		}

	}

	public void PreviousTip ()
	{
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().Play();

		switch (GameState.state) {
		case 1:

				if (Camera.main.GetComponent<Sacrifice>().scoreCount < storeMinimum){

				Debug.Log("currentBloodTip= " + currentBloodTip);
					currentBloodTip--;

					bloodTipObj.GetComponent<FadeTMP>().FadeOut();
				} else {

					currentStoreTip--;
					storeTipObj.GetComponent<FadeTMP>().FadeOut();
				}

			break;
		case 2:
			if (currentBrokerTip > 0) {	
				currentBrokerTip--;
				brokerTipObj.GetComponent<FadeTMP>().FadeOut();
			}
			break;
		case 3:
			if (currentClawTip > 0) {	
				currentClawTip--;
				clawTipObj.GetComponent<FadeTMP>().FadeOut();
			} 
			break;
		}


	}
}