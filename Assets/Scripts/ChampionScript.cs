﻿using UnityEngine;
using System.Collections;

public class ChampionScript : MonoBehaviour {

	private CharacterMotor motor;

	public Vector3 cameraDisplacement;
	public float cameraFollowSpeed;

	private Vector3 directionVector;

	// Use this for initialization
	void Start () {
		motor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update () {
		if(networkView.isMine) {
			// Get the input vector from keyboard or analog stick
			directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

			//Make camera follow champion
			Vector3 cameraTarget = transform.position + cameraDisplacement;
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTarget, cameraFollowSpeed*Time.deltaTime);
			Camera.main.transform.LookAt(transform.position);
		}

		if(directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = directionVector;
	}


	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if(stream.isWriting) {
			Vector3 position = transform.position; stream.Serialize(ref position);
			stream.Serialize(ref directionVector);
		}
		else {
			Vector3 position = Vector3.zero; stream.Serialize(ref position);
			transform.position = position;

			stream.Serialize(ref directionVector);
		}
	}
}
