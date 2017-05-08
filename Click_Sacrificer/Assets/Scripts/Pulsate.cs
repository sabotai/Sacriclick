using UnityEngine;
using System.Collections;

public class Pulsate : MonoBehaviour {

	public Vector3 startScale;
	public Vector3 endScale;
	public float speed = 1.0F;
	public float startTime;
	public float scaleSize;

	// Use this for initialization
	void Start () {

		startScale = transform.localScale;
		endScale = transform.localScale *= 0.75f;
		startTime = Time.time;
		scaleSize = Vector3.Distance(startScale, endScale);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	void Pulse(GameObject pulser){
		if (Input.GetMouseButton(0)){
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / scaleSize;
			pulser.transform.localScale = Vector3.Lerp(pulser.transform.localScale, pulser.transform.localScale * 0.5f, fracJourney);
			//pulser.transform.localScale *= 0.75f;
		} else {
			pulser.transform.localScale *= 1.25f;
		}
	}
	*/

	public static IEnumerator Pulse(GameObject pulser, float duration, float magnitude) {
		Debug.Log ("pulsing...");


		float elapsed = 0.0f;

		Vector3 origScale = pulser.transform.localScale;

		while (elapsed < duration) {

			elapsed += Time.deltaTime;          

			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;

			pulser.transform.localScale = new Vector3(x + origScale.x, y + origScale.y, origScale.z); 

			yield return null;
		}

		pulser.transform.localScale = origScale;
	}


	//alternative method for predefining return scale
	public static IEnumerator Pulse(GameObject pulser, float duration, float magnitude, Vector3 definedScale) {
		Debug.Log ("pulsing...");


		float elapsed = 0.0f;

		Vector3 origScale = definedScale;

		while (elapsed < duration) {

			elapsed += Time.deltaTime;          

			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;

			pulser.transform.localScale = new Vector3(x + origScale.x, y + origScale.y, origScale.z); 

			yield return null;
		}

		pulser.transform.localScale = definedScale;
	}
}
