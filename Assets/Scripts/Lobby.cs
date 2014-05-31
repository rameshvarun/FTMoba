using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Lobby : MonoBehaviour {
	public MatchConfig config;

	// Use this for initialization
	void Start () {
		MatchConfig.singleton = new MatchConfig();
		config = MatchConfig.singleton;

		if (Network.isServer) //If you are the server, add yourself to the game
			AddPlayer (Network.player, (int)config.nextTeam());
		else //If you are the client, tell the server that you have connected
			networkView.RPC ("PlayerConnected", RPCMode.Server, Network.player);

		config = MatchConfig.singleton;
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

	[RPC]
	void PlayerConnected(NetworkPlayer newPlayer){
		//Bring this player up to date with all of the current players in the match config
		foreach (NetworkPlayer player in config.getPlayersOnTeam(Team.All)) {
			networkView.RPC ("AddPlayer", newPlayer, player, (int)(config.getTeam (player)) );
		}

		networkView.RPC("AddPlayer", RPCMode.All, newPlayer, (int)(config.nextTeam()) );
	}

	void OnPlayerDisconnected(NetworkPlayer leavingPlayer){
		//Remove this player from everybody's config
		networkView.RPC ("RemovePlayer", RPCMode.All, leavingPlayer);
	}

	List<string> messages = new List<string>();

	[RPC]
	void RemovePlayer(NetworkPlayer player){ config.RemovePlayer(player); }

	[RPC]
	void AddPlayer(NetworkPlayer player, int team){
		config.AddPlayer((Team)team, player);
	}

	[RPC]
	void AddMessage(string message) { messages.Add(message); }

	[RPC]
	void setRole(NetworkPlayer player, int role){
		config.setRole(player, (Role)role);
	}

	[RPC]
	void startLevel() {
		Network.isMessageQueueRunning = false;
		Application.LoadLevel (2);
		Network.isMessageQueueRunning = true;
	}

	Vector2 chatScroll = new Vector2();
	string chat = "";

	void OnGUI() {
		GUILayout.Label("Hosing On: " + Network.player.ipAddress + ":" + Network.player.port);

		chatScroll = GUILayout.BeginScrollView(chatScroll);
		foreach(string message in messages) {
			GUILayout.Label(message);
		}
		GUILayout.EndScrollView();

		chat = GUILayout.TextField(chat);
		if(GUILayout.Button("Send Message")) {
			networkView.RPC("AddMessage", RPCMode.All, Network.player.ToString() + " says: " + chat);
			chat = "";
		}

		GUILayout.Label ("Red Team");
		foreach (NetworkPlayer redPlayer in config.getPlayersOnTeam(Team.Red) ) {
			GUILayout.Label("player " + redPlayer.ToString ());
		}

		GUILayout.Label ("Blue Team");

		foreach (NetworkPlayer bluePlayer in config.getPlayersOnTeam(Team.Blue) ) {
			GUILayout.Label("player " + bluePlayer.ToString());
		}

		if(GUILayout.Button("Leave")) {
			Network.Disconnect();
			Application.LoadLevel(0);
		}

		if (config.readyToStart() && Network.isServer) {
			if (GUILayout.Button ("Start Game")) {
				try {
					networkView.RPC("setRole", RPCMode.All, config.randomPlayer(Team.Red), (int)Role.DungeonMaster);
					networkView.RPC("setRole", RPCMode.All, config.randomPlayer(Team.Blue), (int)Role.DungeonMaster);
				} catch (Exception e) {
					System.Console.Out.WriteLine(e.StackTrace);
				}

				networkView.RPC("startLevel", RPCMode.All);
			}
		}
	}
}
