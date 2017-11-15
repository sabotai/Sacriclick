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
    public float smoothTime = 0.5F;
    private float yVelocity = 0.0F;
    float pPct = 0f;
    public bool manControl = false;

	// Use this for initialization
	void Start () {
		defaultRot = new Vector3 (-23, -99, -48);
		sun = GetComponent<Light>();
		if (setDefRot)
			transform.rotation = Quaternion.Euler (defaultRot);

	}
	
	// Update is called once per frame
	void Update () {
		/* old way

		float rot = Time.deltaTime * speedMult;
		transform.Rotate (0, 0, rot, Space.World);

		//z rotation of 0 = completely bright, z rotation
		float intens = (Mathf.Abs(transform.localEulerAngles.z - 180)/180);
		sun.intensity = Mathf.Clamp(intens * 2f, 0.5f, 1f);
		sun.shadowStrength = intens;
		//transform.LookAt(sunObj.transform);
		*/

		//transform.Rotate (0, 0, Time.deltaTime * 10f, Space.World); //some really slow movement for the cloud cookie to appear as if moving
		rotateSun(rotAmt);
	}

	public void rotateSun(float pct){
        float softPct = Mathf.SmoothDamp(pPct, pct, ref yVelocity, smoothTime);
       

		transform.rotation = Quaternion.Slerp(brightRot.rotation, darkRot.rotation, softPct);

		//z rotation of 0 = completely bright, z rotation
		float intens = 1f - softPct;
		sun.intensity = Mathf.Clamp(intens, 0.5f, 1f);
		sun.shadowStrength = intens;
		//transform.LookAt(sunObj.transform);
		pPct = pct;
	}
}