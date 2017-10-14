using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	public float rotateSpeed = 5f;
	public Vector3 direction;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        // Rotate the object around its local X axis at 1 degree per second
        transform.Rotate(direction.x * Time.deltaTime * rotateSpeed,direction.y * Time.deltaTime * rotateSpeed,direction.z * Time.deltaTime * rotateSpeed);
	}
}
