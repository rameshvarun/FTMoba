using UnityEngine;
using System.Collections;

public class LinearBullet : MonoBehaviour {

	public float velocity;
	public float lifetime;

	private float time;

	public IUnit owner;

	// Use this for initialization
	void Start () {
		time = 0;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * velocity*Time.deltaTime;
		time += Time.deltaTime;

		if(time > lifetime)
			Network.Destroy(gameObject);
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
