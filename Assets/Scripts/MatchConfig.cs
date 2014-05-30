using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enumeration for teams.
/// </summary>
public enum Team {
	Red,
	Blue,
	Spectator,
	Both,
	All
}

/// <summary>
/// Enumeration for the roles that different players can take.
/// </summary>
public enum Role {
	DungeonMaster,
	Champion,
	Spectator
}

public class MatchConfig {
	private NetworkPlayer? redDM;
	private NetworkPlayer? blueDM;

	private List<NetworkPlayer> redTeam = new List<NetworkPlayer>();
	private List<NetworkPlayer> blueTeam = new List<NetworkPlayer>();
	private List<NetworkPlayer> spectators = new List<NetworkPlayer>();

	/// <summary>
	/// Get a list of players on the given team.
	/// </summary>
	/// <returns>The team.</returns>
	/// <param name="team">Team.</param>
	public List<NetworkPlayer> getPlayersOnTeam(Team team) {
		switch(team) {
		case Team.Red:
			return redTeam;
		case Team.Blue:
			return blueTeam;
		case Team.Spectator:
			return spectators;
		case Team.Both:
			List<NetworkPlayer> both = new List<NetworkPlayer>();
			both.AddRange(redTeam);
			both.AddRange(blueTeam);
			return both;
		case Team.All:
			List<NetworkPlayer> all = new List<NetworkPlayer>();
			all.AddRange(redTeam);
			all.AddRange(blueTeam);
			all.AddRange(spectators);
			return all;
		default:
			return null;
		}
	}

	/// <summary>
	/// Gets the dungeon master for the given team.
	/// </summary>
	/// <returns>The dungeon master.</returns>
	/// <param name="team">Team.</param>
	public NetworkPlayer? getDungeonMaster(Team team) {
		switch(team) {
			case Team.Red:
				return redDM;
			case Team.Blue:
				return blueDM;
			default:
				return null;
		}
	}

	/// <summary>
	/// Returns the role of the given player.
	/// </summary>
	/// <returns>The role.</returns>
	/// <param name="player">Player.</param>
	public Role getRole(NetworkPlayer player) {
		Team team = getTeam(player);
		if(team == Team.Spectator)
			return Role.Spectator;
		else
			return (getDungeonMaster(team) == player) ? Role.DungeonMaster : Role.Champion;
	}

	/// <summary>
	/// Gets the team of the provided player.
	/// </summary>
	/// <returns>The team.</returns>
	/// <param name="player">Player.</param>
	public Team getTeam(NetworkPlayer player) {
		if(redTeam.Contains(player)) return Team.Red;
		if(blueTeam.Contains(player)) return Team.Blue;
		else return Team.Spectator;
	}

	/// <summary>
	/// Determines whether or not the game is ready to start.
	/// </summary>
	/// <returns><c>true</c>, if ready to start, <c>false</c> otherwise.</returns>
	public bool readyToStart() {
		return true; //TODO: Determine if there are at least two players on each team
	}

	/// <summary>
	/// Adds a player to the given team.
	/// </summary>
	/// <param name="team">Team.</param>
	/// <param name="player">Player.</param>
	public void AddPlayer(Team team, NetworkPlayer player) {
		getPlayersOnTeam(team).Add(player);
	}

	/// <summary>
	/// Remove a given player entirely from the match configuration.
	/// </summary>
	/// <param name="player">Player.</param>
	public void RemovePlayer(NetworkPlayer player) {
		getPlayersOnTeam(getTeam(player)).Remove(player);
	}

	/// <summary>
	/// Returns what team the next player should be placed on.
	/// </summary>
	/// <returns>The team.</returns>
	public Team nextTeam() {
		if(redTeam.Count == blueTeam.Count)
			return (Utils.RandomChance()) ? Team.Red : Team.Blue;

		if(redTeam.Count < blueTeam.Count)
			return Team.Red;
		else
			return Team.Blue;
	}

	public void setRole(NetworkPlayer player, Role role) {
		Team team = getTeam(player);
		if(role == Role.Champion && getDungeonMaster(team) == player) {
			if(team == Team.Red) redDM = null;
			else blueDM = null;
		}

		if(role == Role.DungeonMaster) {
			if(team == Team.Red) redDM = player;
			else blueDM = player;
		}

		//TODO: If role is spectator
	}

	public NetworkPlayer randomPlayer(Team team) {
		List<NetworkPlayer> players = getPlayersOnTeam(team);
		int index = Random.Range(0, players.Count);
		return players[index];
	}
	
	/// <summary>
	/// Singleton instance of the MatchConfig class.
	/// </summary>
	public static MatchConfig singleton;
}