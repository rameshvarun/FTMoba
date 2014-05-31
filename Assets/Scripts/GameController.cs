using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Transform championPrefab;

	private MatchConfig config;

	// Use this for initialization
	void Start () {
		config = MatchConfig.singleton;

		Spawn ();
	}

	void Spawn() {
		//if(config.getRole(Network.player) == Role.Champion) {
		GameObject spawnObject = GameObject.Find( (config.getTeam(Network.player) == Team.Red) ? "RedSpawn" : "BlueSpawn" );
		Network.Instantiate(championPrefab, spawnObject.transform.position, spawnObject.transform.rotation, 0);
		//}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		ConnectToServer.disconnection = info;
		
		Debug.Log("Disconnected from server: " + info);
		ConnectToServer.states.Push(ConnectToServer.MenuState.Disconnection);
		Application.LoadLevel(0);
	}
}
