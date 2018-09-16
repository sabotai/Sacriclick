using UnityEngine;
using System.Collections;

public class RFX4_DeactivateByTime_Crane : MonoBehaviour {

    public float DeactivateTime = 3;

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
        }
    }

    // Update is called once per frame
    void DeactivateThis()
    {
        gameObject.SetActive(false);
        CraneGame.beginCraneGame = true;
	}
}
