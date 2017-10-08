using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock : MonoBehaviour {
    public float movingSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 currentPosition = gameObject.transform.position;
        //gameObject.transform.Translate(currentPosition.x, currentPosition.y * movingSpeed * Time.deltaTime, currentPosition.z);
        gameObject.transform.position = new Vector3(currentPosition.x, (float)-1.08 + (Mathf.Sin(Time.time * (movingSpeed))/10), currentPosition.z);
    }
}
