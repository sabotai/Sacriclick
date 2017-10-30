using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursword : MonoBehaviour {

	public GameObject swordObj;
	//public bool raycastMethod = false;
	public Vector3 offset = new Vector3(0f,0f,0f);
	public bool easeMovement = true;
	public float easeAmt = 0.03f;
	float yVelocity = 0.0F;

	public bool scaleSword = true;
	public float swordScaleDecay = 0.005f;
	Vector3 prevCursor;
	public Vector3 maxSize;
	public Vector3 minSize;
	public bool hideCursor = true;
	[System.NonSerialized] public float currentSize = 0f;
	public Color defaultSwordColor;
	public Color greenSwordColor;

	// Use this for initialization
	void Start () {
		if (hideCursor) Cursor.visible = false;
		if (maxSize == null) maxSize = transform.localScale;
		if (minSize == null) minSize = new Vector3(0.1f,0.1f,0.1f);
		currentSize = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {

		if (Camera.main.gameObject.GetComponent<Drag>().panMode) { 
			hideCursor = true; 
			GetComponent<SpriteRenderer> ().enabled = true;
		} else { 
			hideCursor = false; 
			GetComponent<SpriteRenderer> ().enabled = false;
		}
			

		if (hideCursor) {
			Cursor.visible = false;

		} else {
			Cursor.visible = true;
		}

		if (Camera.main.gameObject.GetComponent<Drag>().dragItem != null || Camera.main.gameObject.GetComponent<Drag>().hoverItem != null){
			GetComponent<SpriteRenderer> ().material.color = greenSwordColor;
		} else {
			GetComponent<SpriteRenderer> ().material.color = defaultSwordColor;
		}

		//if (raycastMethod){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit, 1000f,  LayerMask.GetMask("sword-ui"))){
            	//Debug.Log("sword to ... " + rayHit.transform.gameObject.name);
            	Vector3 destination = new Vector3(rayHit.point.x, rayHit.point.y, rayHit.point.z) + offset;
				if (easeMovement){ //&& Vector3.Distance(swordObj.transform.position, destination) > 0.1f) {
            		SmoothMove(swordObj.transform, destination, easeAmt);
            	} else {
                	swordObj.transform.position = destination;
            	}
            }

           if (scaleSword){
           		float distMove = Mathf.Abs(Vector3.Distance(Input.mousePosition, prevCursor));
           		if (distMove > 3f){
					//currentSize += swordScaleDecay * Time.deltaTime;
					
				if (currentSize < 0.1f) currentSize += 0.0001f;
					currentSize *= 1f + (swordScaleDecay * Time.deltaTime);
           		} else {
					//currentSize -= swordScaleDecay * Time.deltaTime;
					currentSize *= 1f - (swordScaleDecay * Time.deltaTime);
           		}
           		currentSize = Mathf.Clamp(currentSize, 0f, 1f);
				//float lerpAmt = Mathf.Lerp( -1f, 1f, distMove / 200f);

				//Debug.Log(distMove + " distance and " + lerpAmt);
				Vector3 interScale = Vector3.Lerp(minSize, maxSize, currentSize);
				//if (easeMovement){// && Vector3.Distance(swordObj.transform.localScale, interScale) > 0.1f){
				//	SmoothScale(transform, interScale, easeAmt);
				//} else {
				transform.localScale = interScale;
	           		//if ()
				//}
           }

/*
        } else {
            	Vector3 scaledMouse = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, swordObj.transform.position.z);
            	scaledMouse = Vector3.Scale(scaledMouse, new Vector3 ( 13f, 13, 0f));
            	swordObj.transform.localPosition = scaledMouse + offset;



        }
*/
        prevCursor = Input.mousePosition;
	}

	public void SmoothMove(Transform moveMe, Vector3 targett, float smoothTtime){


		float newPositionX = Mathf.SmoothDamp(moveMe.position.x, targett.x, ref yVelocity, smoothTtime);
		float newPositionY = Mathf.SmoothDamp(moveMe.position.y, targett.y, ref yVelocity, smoothTtime);
		float newPositionZ = Mathf.SmoothDamp(moveMe.position.z, targett.z, ref yVelocity, smoothTtime);
		moveMe.position = new Vector3(newPositionX, newPositionY, moveMe.position.z);
		//moveMe.position = new Vector3(newPositionX, newPositionY, newPositionZ);
	}
	public void SmoothScale(Transform scaleMe, Vector3 targett, float smoothTtime){
		float newScale = Mathf.SmoothDamp(scaleMe.localScale.x, targett.x, ref yVelocity, smoothTtime);
		//float newScaleX = Mathf.SmoothDamp(scaleMe.position.y, targett.y, ref yVelocity, smoothTtime);
		//float newScaleX = Mathf.SmoothDamp(scaleMe.position.z, targett.z, ref yVelocity, smoothTtime);
		scaleMe.localScale = new Vector3(newScale, newScale, newScale);
		//moveMe.position = new Vector3(newPositionX, newPositionY, newPositionZ);
	}
}
