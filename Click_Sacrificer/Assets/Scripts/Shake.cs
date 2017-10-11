
using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour {
	//generic shake script

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public static IEnumerator ShakeThis(Transform shaked, float duration, float magnitude) {
		//Debug.Log ("shaking...");


		float elapsed = 0.0f;

		Vector3 originalPos = shaked.position;

		while (elapsed < duration) {

			elapsed += Time.deltaTime;          

			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;

			shaked.position = new Vector3(x + originalPos.x, y + originalPos.y, originalPos.z); 

			yield return null;
		}

		shaked.position = originalPos;
	}

}