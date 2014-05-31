using UnityEngine;
using System.Collections;

/// <summary>
/// Base class that manages how the match state is synchronized. Both the Lobby and GameController class inherit from this.
/// </summary>
public class MatchManagement : MonoBehaviour
{
	public MatchConfig config;

	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		ConnectToServer.disconnection = info;
		
		Debug.Log("Disconnected from server: " + info);
		ConnectToServer.states.Push(ConnectToServer.MenuState.Disconnection);
		Application.LoadLevel(0);
	}

	public void OnPlayerDisconnected(NetworkPlayer leavingPlayer){
		//Remove this player from everybody's config
		networkView.RPC ("RemovePlayer", RPCMode.All, leavingPlayer);
	}

	[RPC]
	public void RemovePlayer(NetworkPlayer player){ config.RemovePlayer(player); }
	
	[RPC]
	public void AddPlayer(NetworkPlayer player, int team){
		config.AddPlayer((Team)team, player);
	}
	
	[RPC]
	public void setRole(NetworkPlayer player, int role){
		config.setRole(player, (Role)role);
	}

	[RPC]
	public void PlayerConnected(NetworkPlayer newPlayer){
		//Bring this player up to date with all of the current players in the match config
		foreach (NetworkPlayer player in config.getPlayersOnTeam(Team.All)) {
			networkView.RPC ("AddPlayer", newPlayer, player, (int)(config.getTeam (player)) );
		}
		
		networkView.RPC("AddPlayer", RPCMode.All, newPlayer, (int)(config.nextTeam()) );
	}
}

