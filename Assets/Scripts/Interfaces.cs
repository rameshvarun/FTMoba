using UnityEngine;
using System.Collections;

/// <summary>
/// Interface that should capture all units. This includes:
/// Champions
/// Towers
/// Creeps
/// </summary>
public interface IUnit
{
	/// <summary>
	/// Return the team that this unit is on.
	/// </summary>
	/// <returns>The team.</returns>
	Team getTeam();
}

