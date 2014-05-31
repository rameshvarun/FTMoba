using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ConnectToServer : MonoBehaviour {

	/// <summary>
	/// This field should be populated with the latest disconnection error, to be displayed in the correct menu state.
	/// </summary>
	public static NetworkDisconnection disconnection;

	/// <summary>
	/// An enumeration that stores all possible states that the menu can be in.
	/// </summary>
	public enum MenuState {
		JoinOrHost,
		JoinOptions,
		HostOptions,

		EnterIPAdress,
		JoinOnlineGame,
		JoinLANGame,
		Disconnection
	}

	/// <summary>
	/// The menu states are stored in a stack. Entering a new menu pushes a state onto the stack, hitting the back button pops a state off.
	/// </summary>
	public static Stack<MenuState> states = new Stack<MenuState>();

	void Awake() {
		MasterServer.RequestHostList("FuckThisMOBA");
		Application.runInBackground = true;
	}

	// Use this for initialization
	void Start () {
		//If there are no states, set it to the current menu
		if(states.Count == 0) states.Push(MenuState.JoinOrHost);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnServerInitialized() {
		Debug.Log("Server successfully initialized.");
		Application.LoadLevel(1);
	}

	void OnConnectedToServer() {
		Debug.Log("Successfully connected to remote server.");
		Application.LoadLevel(1);
	}

	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
	}

	string ipaddressfield = "";
	void OnGUI(){
		if(states.Peek() == MenuState.JoinOrHost) {
			if (GUILayout.Button ("Join Game"))
			{
				states.Push( MenuState.JoinOptions );
			}

			if (GUILayout.Button ("Host Game"))
			{
				states.Push( MenuState.HostOptions );
			}
		}

		if(states.Peek() == MenuState.JoinOptions) {
			
			if (GUILayout.Button ("Enter IP Address"))
			{
				states.Push( MenuState.EnterIPAdress);
			}

			if (GUILayout.Button ("Join Online Game"))
			{
				states.Push( MenuState.JoinOnlineGame );
			}

			if (GUILayout.Button ("Join LAN Game"))
			{
				states.Push( MenuState.JoinLANGame );
			}
		}
	
		if(states.Peek() == MenuState.HostOptions) {
			
			if (GUILayout.Button ("Start Local Server"))
			{
				Network.InitializeServer(32, 9050, false);
			}

			if (GUILayout.Button ("Start Online Server"))
			{
				Network.InitializeServer(32, 9050, !Network.HavePublicAddress());
				MasterServer.RegisterHost("FuckThisMOBA", "Test Game", "Test Game Description");
			}
		}

		if(states.Peek() == MenuState.EnterIPAdress) {
			ipaddressfield = GUILayout.TextField(ipaddressfield);
			if(GUILayout.Button("Connect")) {
				string[] ipstrings = ipaddressfield.Split(':');
				string ipaddress = ipstrings[0];
				int port = 9050;
				if(ipstrings.Length > 1)
					port = Int32.Parse(ipstrings[1]);

				Network.Connect(ipaddress, port);
			}
		}

		if(states.Peek() == MenuState.JoinOnlineGame) {
			HostData[] data = MasterServer.PollHostList();

			// Go through all the hosts in the host list
			foreach (HostData element in data)
			{
				GUILayout.BeginHorizontal();	
				string name = element.gameName + " " + element.connectedPlayers + " / " + element.playerLimit;
				GUILayout.Label(name);	
				GUILayout.Space(5);

				string hostInfo;
				hostInfo = "[";
				foreach (string host in element.ip)
					hostInfo = hostInfo + host + ":" + element.port + " ";
				hostInfo = hostInfo + "]";
				GUILayout.Label(hostInfo);	
				GUILayout.Space(5);
				GUILayout.Label(element.comment);
				GUILayout.Space(5);
				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Connect"))
				{
					// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
					Network.Connect(element);			
				}
				GUILayout.EndHorizontal();	
			}

			if(GUILayout.Button("Refresh")) {
				MasterServer.RequestHostList("FuckThisMOBA");
			}
		}

		if(states.Peek() == MenuState.Disconnection) {
			GUILayout.Label("Disconnected from server: " + disconnection);
		}

		if(states.Count > 1) {
			if(GUILayout.Button("Back")) {
				states.Pop();
			}
		}
	}
}
