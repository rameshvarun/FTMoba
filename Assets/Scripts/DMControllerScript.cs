using UnityEngine;
using System.Collections;

public class DMControllerScript : MonoBehaviour {

	private GameObject looktarget;

	// Use this for initialization
	void Start () {
		looktarget = GameObject.Find("DMLookTarget");

		Camera.main.transform.position = transform.position;
		Camera.main.transform.LookAt(looktarget.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
