using UnityEngine;
using System.Collections;

/// <summary>
/// Simple enumeration to differentiate between the three lanes.
/// </summary>
[System.Serializable]
public enum Lane {
	Top, Middle, Bottom
}

public class CreepScript : MonoBehaviour, IUnit {

	private int currentWaypoint = 0;

	public Lane lane;

	public float rotateSpeed;

	private CharacterMotor motor;

	private GameController gameController;

	public Team team;
	public Team getTeam() { return team; }

	public Bar healthBar;

	private float health;
	public float maxHealth;

	public bool isAlive() {
		return health > 0;
	}

	public void Attack(float power) {}

	// Use this for initialization
	void Start () {
		motor = GetComponent<CharacterMotor>();
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

		health = maxHealth;
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if(stream.isWriting) {
			//Position Synchronization
			Vector3 position = transform.position; stream.Serialize(ref position);

			//Rotation Synchronization
			Quaternion rotation = transform.rotation; stream.Serialize(ref rotation);

			//Current Movement
			Vector3 directionVector = motor.inputMoveDirection;
			stream.Serialize(ref directionVector);
		}
		else {
			//Position Synchronization
			Vector3 position = Vector3.zero; stream.Serialize(ref position);
			transform.position = position;

			//Rotation synchronization
			Quaternion rotation = Quaternion.identity; stream.Serialize(ref rotation);
			transform.rotation = rotation;
			
			//Current Movement
			Vector3 directionVector = Vector3.zero;
			stream.Serialize(ref directionVector);
			motor.inputMoveDirection = directionVector;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(networkView.isMine) {
			Transform[] path = null;

			switch(lane) {
				case Lane.Middle:
					path = gameController.paths.middlePath; break;
				case Lane.Top:
					path = gameController.paths.topPath; break;
				case Lane.Bottom:
					path = gameController.paths.bottomPath; break;
			}

			if(currentWaypoint < path.Length) {
				Transform waypoint = path[currentWaypoint];
				Vector3 targetPosition = new Vector3(waypoint.position.x, transform.position.y, waypoint.position.z);

				Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
				transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

				motor.inputMoveDirection = transform.rotation * new Vector3(0, 0, 1);

				if(Vector3.Distance(targetPosition, transform.position) < 1.0f) {
					currentWaypoint++;
				}
			}
			else {
				motor.inputMoveDirection = Vector3.zero;
			}
		}
	}

	void OnGUI() {
		//Health bar position
		Vector3 characterScreenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2, 0));
		characterScreenPosition.y = Screen.height - characterScreenPosition.y;
		
		//Draw healthbar
		healthBar.position = new Vector2(characterScreenPosition.x, characterScreenPosition.y);
		healthBar.SetPercentage(health / maxHealth);
		healthBar.OnGUI();
	}
}
