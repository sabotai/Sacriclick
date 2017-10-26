using UnityEngine;
using System.Collections;

public class SmoothShift : MonoBehaviour {

    public Transform target;
    public float smoothTime = 0.3F;
    private float yVelocity = 0.0F;


    void Update() {
        float newPositionX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref yVelocity, smoothTime);
        float newPositionY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref yVelocity, smoothTime);
        float newPositionZ = Mathf.SmoothDamp(transform.position.z, target.position.z, ref yVelocity, smoothTime);
        transform.position = new Vector3(newPositionX, newPositionY, newPositionZ);
    }

    public static void SmoothMove(Transform moveMe, Vector3 targett, float smoothTtime){

	   
	    float yVelocity = 0.0F;

        float newPositionX = Mathf.SmoothDamp(moveMe.position.x, targett.x, ref yVelocity, smoothTtime);
        float newPositionY = Mathf.SmoothDamp(moveMe.position.y, targett.y, ref yVelocity, smoothTtime);
        float newPositionZ = Mathf.SmoothDamp(moveMe.position.z, targett.z, ref yVelocity, smoothTtime);
        moveMe.position = new Vector3(newPositionX, newPositionY, newPositionZ);
    }
}