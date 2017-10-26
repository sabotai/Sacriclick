using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursword : MonoBehaviour {

	public GameObject swordObj;
	public bool raycastMethod = false;
	public Vector3 offset = new Vector3(0f,0f,0f);
	public bool easeMovement = true;
	public bool scaleSword = true;
	Vector3 prevCursor;
	public Vector3 maxSize;
	public Vector3 minSize;
	float currentSize = 0f;

	// Use this for initialization
	void Start () {
		//Cursor.visible = false;
		if (maxSize == null) maxSize = transform.localScale;
		if (minSize == null) minSize = new Vector3(0.1f,0.1f,0.1f);
		currentSize = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {

		if (raycastMethod){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit, 1000f,  LayerMask.GetMask("sword-ui"))){
            	//Debug.Log("sword to ... " + rayHit.transform.gameObject.name);
            	Vector3 destination = new Vector3(rayHit.point.x, rayHit.point.y, rayHit.point.z) + offset;
            	if (easeMovement) {
            		SmoothMove(swordObj.transform, destination, 0.03f);
            	} else {
                	swordObj.transform.position = destination;
            	}
            }

           if (scaleSword){
           		float distMove = Mathf.Abs(Vector3.Distance(Input.mousePosition, prevCursor));
           		if (distMove > 5f){
					currentSize += 0.01f;
           		} else {
           			currentSize -= 0.01f;
           		}
           		currentSize = Mathf.Clamp(currentSize, 0f, 1f);
				//float lerpAmt = Mathf.Lerp( -1f, 1f, distMove / 200f);

				//Debug.Log(distMove + " distance and " + lerpAmt);
           		transform.localScale = Vector3.Lerp(minSize, maxSize, currentSize);
           		//if ()
           }

        } else {
            	Vector3 scaledMouse = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, swordObj.transform.position.z);
            	scaledMouse = Vector3.Scale(scaledMouse, new Vector3 ( 13f, 13, 0f));
            	swordObj.transform.localPosition = scaledMouse + offset;



        }

        prevCursor = Input.mousePosition;
	}

    public void SmoothMove(Transform moveMe, Vector3 targett, float smoothTtime){

	   
	    float yVelocity = 0.0F;

        float newPositionX = Mathf.SmoothDamp(moveMe.position.x, targett.x, ref yVelocity, smoothTtime);
        float newPositionY = Mathf.SmoothDamp(moveMe.position.y, targett.y, ref yVelocity, smoothTtime);
        float newPositionZ = Mathf.SmoothDamp(moveMe.position.z, targett.z, ref yVelocity, smoothTtime);
        moveMe.position = new Vector3(newPositionX, newPositionY, moveMe.position.z);
        //moveMe.position = new Vector3(newPositionX, newPositionY, newPositionZ);
    }
}
