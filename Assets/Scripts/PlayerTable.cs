using UnityEngine;
using System.Collections;

public class PlayerTable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnGUI() {
		if(Input.GetKey(KeyCode.Return)) {
			int TABLE_WIDTH = 500;
			int TABLE_HEIGHT = 500;
			GUILayout.BeginArea(new Rect(Screen.width/2 - TABLE_WIDTH/2, Screen.height/2 - TABLE_HEIGHT/2, TABLE_WIDTH, TABLE_HEIGHT));
			GUILayout.BeginVertical("box");

			GUILayout.BeginHorizontal();
			GUILayout.Label("Player");
			GUILayout.Label("Avg Ping");
			GUILayout.Label("Last Ping");
			GUILayout.EndHorizontal();

			foreach(NetworkPlayer player in Network.connections) {
				GUILayout.BeginHorizontal();
				GUILayout.Label("Player " + player.ToString());
				GUILayout.Label(Network.GetAveragePing(player).ToString());
				GUILayout.Label(Network.GetLastPing(player).ToString());

				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
}
