using UnityEngine;
using System.Collections;

public class RFX4_DeactivateByTime_Crane : MonoBehaviour {

    public float DeactivateTime = 3f;
    public float beginCrane = 1f;

    private bool canUpdateState;
	// Use this for initialization
	void OnEnable ()
	{
	    canUpdateState = true;
	}

    private void Update()
    {
        if (canUpdateState) {
            canUpdateState = false;
            Invoke("DeactivateThis", DeactivateTime);
            Invoke("CraneBegin", beginCrane);
        }
    }

    // Update is called once per frame
    void DeactivateThis()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void CraneBegin()
    {
        CraneGame.beginCraneGame = true;
    }
}
