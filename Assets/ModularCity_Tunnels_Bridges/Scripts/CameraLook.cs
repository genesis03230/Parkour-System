using UnityEngine;

using System.Collections;



public class CameraLook : MonoBehaviour 

{

	//    public Camera  cameraToLookAt;
	private Transform target;

	void Start () {

		target = GameObject.FindWithTag("Player").transform;
	}

	void Update () 

	{

		Vector3  v = target.transform.position  - transform.position ;

		v.x = v.z = 0.0f;

		transform.LookAt (target.transform.position  - v); 

	}

}