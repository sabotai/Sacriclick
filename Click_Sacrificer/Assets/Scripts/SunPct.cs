using UnityEngine;
using System.Collections;

public class SunPct : MonoBehaviour {

	public float speedMult = 1f;
	private Vector3 defaultRot;
	public bool setDefRot = false;
	Light sun;
	public Transform brightRot;
	public Transform darkRot;
	public float rotAmt = 0f;
    float pPct = 0f;
    public bool manControl = false;
    float amt, lightAmt;
    public bool oscillate = false;
    public Vector3 rotDir;

	// Use this for initialization
	void Start () {
		defaultRot = new Vector3 (-23, -99, -48);
		sun = GetComponent<Light>();
		if (setDefRot)
			transform.rotation = Quaternion.Euler (defaultRot);
		amt = 0.01f;
		lightAmt = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!CraneGame.beginCraneGame) rotateSun(rotAmt);
	}

	public void rotateSun(float pct){
		if (pct > pPct) {
			amt *= 1.1f;
			lightAmt *= 0.995f;
		} else {
			amt *= 0.9f;
			lightAmt *= 1.005f;
		}

			amt = Mathf.Clamp(amt, 0.01f, 1f);
			lightAmt = Mathf.Clamp(lightAmt, 0.5f, 1f);

		sun.intensity = lightAmt;
		sun.shadowStrength = lightAmt;

		if (oscillate){
			transform.rotation = Quaternion.Slerp(brightRot.rotation, darkRot.rotation, amt);



		} else {
			transform.Rotate(rotDir * Time.deltaTime * speedMult * pct);
			//-160 to 160 x = dark
			if ((transform.eulerAngles.x > 0 && transform.eulerAngles.x < 160) || (transform.eulerAngles.x > -360 && transform.eulerAngles.x < -160)){
				sun.intensity += 0.01f;
				sun.shadowStrength += 0.01f;
			} else {
				sun.intensity -= 0.01f;
				sun.shadowStrength -= 0.01f;				
			}
			sun.intensity = Mathf.Clamp(sun.intensity,0.5f, 1f);
			sun.shadowStrength = Mathf.Clamp(sun.shadowStrength,0.5f, 1f);

		}

		if (Time.frameCount % 100 == 0)	pPct = pct;
	}
}