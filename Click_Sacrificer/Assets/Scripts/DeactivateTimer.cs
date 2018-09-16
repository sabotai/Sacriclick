using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateTimer : MonoBehaviour {

    public float DeactivateTime = 6;

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
	}
}
