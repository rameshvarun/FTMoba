using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Lobby : MatchManagement {

	// Use this for initialization
	void Start () {
		MatchConfig.singleton = new MatchConfig();
		config = MatchConfig.singleton;

		if (Network.isServer) //If you are the server, add yourself to the game
			AddPlayer (Network.player, (int)config.nextTeam());
		else //If you are the client, tell the server that you have connected
			networkView.RPC ("PlayerConnected", RPCMode.Server, Network.player);
	}
	
	// Update is called once per frame
	void Update () {
	}

	[RPC]
	void startLevel() {
		Network.isMessageQueueRunning = false;
		Application.LoadLevel (2);
		Network.isMessageQueueRunning = true;
	}

	void OnGUI() {
		GUILayout.Label("Hosting On: " + Network.player.ipAddress + ":" + Network.player.port);

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
