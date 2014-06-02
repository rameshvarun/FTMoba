using UnityEngine;
using System.Collections;

public class GameController : MatchManagement {

	public Transform warrior;
	public Transform dmcontrollerPrefab;

	public Transform creep;

	public Cooldown waveCooldown;
	public Cooldown creepCooldown;

	[System.Serializable]
	public class LanePaths {
		public Transform[] topPath;
		public Transform[] middlePath;
		public Transform[] bottomPath;
	}

	public LanePaths paths;

	// Use this for initialization
	void Start () {
		config = MatchConfig.singleton;

		Spawn ();
	}

	/// <summary>
	/// Spawn the appropriate character controller for this player, given their assigned role in the match.
	/// </summary>
	void Spawn() {
		if(config.getRole(Network.player) == Role.Champion) {
			if(config.getChampion(Network.player) != Champion.Unselected) {
				GameObject spawnObject = GameObject.Find( (config.getTeam(Network.player) == Team.Red) ? "RedSpawn" : "BlueSpawn" );

				//Pick the correct prefab to spawn as
				Transform championPrefab = null;
				switch(config.getChampion(Network.player)) {
				case Champion.Warrior:
					championPrefab = warrior; break;
				}

				if(championPrefab != null)
					Network.Instantiate(championPrefab, spawnObject.transform.position, spawnObject.transform.rotation, 0);
			}
		}
		if(config.getRole(Network.player) == Role.DungeonMaster) {
			GameObject spawnObject = GameObject.Find( (config.getTeam(Network.player) == Team.Red) ? "RedDMPos" : "BlueDMPos" );
			Instantiate(dmcontrollerPrefab, spawnObject.transform.position, Quaternion.identity);
		}
		if(config.getRole (Network.player) == Role.Spectator) {
		}
	}

	void SpawnCreep(Team team, Lane lane) {
		//Spawn Position
		GameObject spawnObject = GameObject.Find( (team == Team.Red) ? "RedSpawn" : "BlueSpawn" );
		Transform creepTransform = (Transform)(Network.Instantiate(creep, spawnObject.transform.position, Quaternion.identity, 0));
		CreepScript creepScript = creepTransform.GetComponent<CreepScript>();
		creepScript.team = team;
		creepScript.lane = lane;
	}
	
	// Update is called once per frame
	void Update () {


		if(Network.isServer) {
			creepCooldown.Update();
			waveCooldown.Update();

			if(waveCooldown.IsReady()) {
				waveCooldown.Reset();
			}

			if(waveCooldown.Active()) {
				if(creepCooldown.IsReady())
				{
					creepCooldown.Reset();

					SpawnCreep(Team.Red, Lane.Top);
					SpawnCreep(Team.Blue, Lane.Top);
					
					SpawnCreep(Team.Red, Lane.Middle);
					SpawnCreep(Team.Blue, Lane.Middle);

					SpawnCreep(Team.Red, Lane.Bottom);
					SpawnCreep(Team.Blue, Lane.Bottom);
				}
			}
		}
	}

	void OnGUI() {
		if(config.getRole(Network.player) == Role.Champion) {
			//If the player has not selected a champion, provide them with the selection dialog
			if(config.getChampion(Network.player) == Champion.Unselected) {
					
				int TABLE_WIDTH = 500;
				int TABLE_HEIGHT = 500;
				GUILayout.BeginArea(new Rect(Screen.width/2 - TABLE_WIDTH/2, Screen.height/2 - TABLE_HEIGHT/2, TABLE_WIDTH, TABLE_HEIGHT));
				GUILayout.BeginVertical("box");

				GUILayout.Label("Select a Champion");

				if(GUILayout.Button("Warrior")) {
					config.setChampion(Network.player, Champion.Warrior);
				}
				if(GUILayout.Button("Marhsal")) {
					config.setChampion(Network.player, Champion.Marshall);
				}
				if(GUILayout.Button("Ogre")) {
					config.setChampion(Network.player, Champion.Ogre);
				}
				if(GUILayout.Button("Mage")) {
					config.setChampion(Network.player, Champion.Mage);
				}
				if(GUILayout.Button("Monk")) {
					config.setChampion(Network.player, Champion.Monk);
				}
				if(GUILayout.Button("Prince")) {
					config.setChampion(Network.player, Champion.Prince);
				}

				Spawn();

				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
		}
	}
}
