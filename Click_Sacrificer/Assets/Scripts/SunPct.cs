using UnityEngine;
using System.Collections;

public class SunPct : MonoBehaviour {

	public float speedMult = 1f;
	private Vector3 defaultRot;
	public bool setDefRot = false;
	Light sun;
	public Transform brightRot;
	public Transform darkRot;

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
		
	}

	void rotateSun(float pct){
		float rot = Time.deltaTime * speedMult;
		transform.Rotate (0, 0, rot, Space.World);
		transform.rotation = Quaternion.Slerp(brightRot.rotation, darkRot.rotation, pct);

		//z rotation of 0 = completely bright, z rotation
		float intens = (Mathf.Abs(transform.localEulerAngles.z - 180)/180);
		sun.intensity = Mathf.Clamp(intens * 2f, 0.5f, 1f);
		sun.shadowStrength = intens;
		//transform.LookAt(sunObj.transform);
	}
}