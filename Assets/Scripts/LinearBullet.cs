using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public IUnit owner;

	public float power;
}

public class LinearBullet : Bullet {

	public float velocity;
	public float lifetime;

	private float time;
	
	// Use this for initialization
	void Start () {
		time = 0;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 oldPosition = transform.position;
		transform.position += transform.forward * velocity*Time.deltaTime;
		time += Time.deltaTime;

		if(time > lifetime)
			Network.Destroy(gameObject);



		if(networkView.isMine) {
			Ray ray = new Ray(oldPosition, transform.position - oldPosition);
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, Vector3.Distance(oldPosition, transform.position))) {
				Debug.Log(hit.collider.name);
				if(hit.collider.CompareTag("Unit")) {

				}
				else {
					Network.Destroy(gameObject);
				}
			}
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if(stream.isWriting) {
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);
		}
		else {
			Vector3 position = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			stream.Serialize(ref position);
			stream.Serialize(ref rotation);

			transform.rotation = rotation;
			//Extrapolate the position forwards a bit
			transform.position = position + transform.forward*velocity*(float)(Network.time - info.timestamp);
		}
	}
}
