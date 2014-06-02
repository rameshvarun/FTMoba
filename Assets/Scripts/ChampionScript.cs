using UnityEngine;
using System.Collections;

public class ChampionScript : MonoBehaviour, IUnit {

	private CharacterMotor motor;

	public Transform bullet;
	public Transform rageBullet;

	public Vector3 cameraDisplacement;

	private Vector3 directionVector;

	[System.Serializable]
	public class CameraZoom {
		public float zoomFactor;

		public float zoomMax;
		public float zoomMin;
		public float zoomSpeed;
	}

	private float heath;
	public float maxHealth;

	public Cooldown attack;
	public Cooldown qSkill;
	public Cooldown eSkill;
	public Cooldown rSkill;

	public Cooldown rageAttack;

	public CameraZoom zoom;

	// Use this for initialization
	void Start () {
		motor = GetComponent<CharacterMotor>();
	}
	
	// Update is called once per frame
	void Update () {

		//Update all cooldown systems
		attack.Update();
		qSkill.Update();
		eSkill.Update();
		rSkill.Update();
		rageAttack.Update();

		if(networkView.isMine) {
			// Get the input vector from keyboard or analog stick
			directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

			//Zooming in/out
			zoom.zoomFactor -= zoom.zoomSpeed*Input.GetAxis("Mouse ScrollWheel");
			zoom.zoomFactor = Mathf.Clamp(zoom.zoomFactor, zoom.zoomMin, zoom.zoomMax);

			//Make camera follow champion
			Camera.main.transform.position = transform.position + new Vector3(cameraDisplacement.x, cameraDisplacement.y*zoom.zoomFactor, cameraDisplacement.z);
			Camera.main.transform.LookAt(transform.position);

			//Bullet firing
			if( Input.GetMouseButton(0) && attack.IsReady() ) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit)) {
					attack.Reset();

					Vector3 target = hit.point;
					target.y = transform.position.y;

					Quaternion bulletDirection = Quaternion.LookRotation(target - transform.position);

					if(eSkill.Active()) { //Spread mode
						Network.Instantiate(bullet, transform.position, bulletDirection * Quaternion.Euler(0, 0, 0) , 0);
						Network.Instantiate(bullet, transform.position, bulletDirection * Quaternion.Euler(0, 15, 0) , 0);
						Network.Instantiate(bullet, transform.position, bulletDirection * Quaternion.Euler(0, -15, 0) , 0);
						Network.Instantiate(bullet, transform.position, bulletDirection * Quaternion.Euler(0, 30, 0) , 0);
						Network.Instantiate(bullet, transform.position, bulletDirection * Quaternion.Euler(0, -30, 0) , 0);
					}
					else { //Regular bullet
						Network.Instantiate(bullet, transform.position, bulletDirection, 0);
					}

				}
			}

			//Q Skill Movement Modifier
			if(qSkill.Active()) {
				motor.SetSprinting(true);
				attack.Update();
			}
			else {
				motor.SetSprinting(false);
			}

			if(rSkill.Active() && rageAttack.IsReady()) {
				float rotationOffset = 90 * Time.time;
				Network.Instantiate(rageBullet, transform.position, Quaternion.Euler(0, rotationOffset, 0) , 0);
				Network.Instantiate(rageBullet, transform.position, Quaternion.Euler(0, rotationOffset + 90, 0)  , 0);
				Network.Instantiate(rageBullet, transform.position, Quaternion.Euler(0, rotationOffset + 180, 0) , 0);
				Network.Instantiate(rageBullet, transform.position, Quaternion.Euler(0, rotationOffset + 270, 0) , 0);

				Network.Instantiate(rageBullet, transform.position, Quaternion.Euler(0, rotationOffset + 45, 0) , 0);
				Network.Instantiate(rageBullet, transform.position, Quaternion.Euler(0, rotationOffset + 90 + 45, 0)  , 0);
				Network.Instantiate(rageBullet, transform.position, Quaternion.Euler(0, rotationOffset + 180 + 45, 0) , 0);
				Network.Instantiate(rageBullet, transform.position, Quaternion.Euler(0, rotationOffset + 270 + 45, 0) , 0);

				rageAttack.Reset();
			}
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

	void OnGUI() {
		float height = 100;
		float width = 800;
		GUILayout.BeginArea(new Rect(Screen.width/2 - width/2, Screen.height - height, width, height));

		GUILayout.BeginHorizontal("box");

		if(GUILayout.Button( new GUIContent("Dash", "Increases movement and fire speed for a short amount of time. Cooldown: 6 seconds" ) )
		   || Input.GetKeyDown(KeyCode.Q) ) {
			if(qSkill.IsReady()) {
				qSkill.Reset();
			}
		}

		if(GUILayout.Button( new GUIContent("Spread", "Provides a spread of bullets for a few seconds." ) )
		   || Input.GetKeyDown(KeyCode.E) ) {
			if(eSkill.IsReady()) {
				eSkill.Reset();
			}
		}

		if(GUILayout.Button( new GUIContent("Spiral", "Shoots out bullets in a spiral pattern." ) )
		   || Input.GetKeyDown(KeyCode.R) ) {
			if(rSkill.IsReady()) {
				rSkill.Reset();
			}
		}

		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	public Team getTeam() {
		return MatchConfig.singleton.getTeam(networkView.owner);
	}
}
