using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class InGameChat : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	List<string> messages = new List<string>();

	[RPC]
	void AddMessage(string message) {
		messages.Add(message);
		chatScroll.y = messages.Count*25;
	}

	Vector2 chatScroll = new Vector2();
	string chat = "";
	void OnGUI () {
		GUILayout.BeginArea(new Rect(0, Screen.height - 300, 400, 300));
		GUILayout.BeginVertical("box");

		chatScroll = GUILayout.BeginScrollView(chatScroll);
		foreach(string message in messages) {
			GUILayout.Label(message);
		}
		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();
		chat = GUILayout.TextField(chat);

		if(GUILayout.Button("Send", GUILayout.Width (95)) || Input.GetKey(KeyCode.Return)) {
			if(chat.Length > 0) {
				networkView.RPC("AddMessage", RPCMode.All, "Player " + Network.player.ToString() + " says: " + chat);
				chat = "";
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
