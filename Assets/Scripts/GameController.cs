using UnityEngine;
using System.Collections;

public class GameController : MatchManagement {

	public Transform championPrefab;
	public Transform dmcontrollerPrefab;

	// Use this for initialization
	void Start () {
		config = MatchConfig.singleton;

		Spawn ();
	}

	void Spawn() {
		if(config.getRole(Network.player) == Role.Champion) {
			if(config.getChampion(Network.player) != Champion.Unselected) {
				GameObject spawnObject = GameObject.Find( (config.getTeam(Network.player) == Team.Red) ? "RedSpawn" : "BlueSpawn" );
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
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if(config.getRole(Network.player) == Role.Champion) {
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

				Spawn ();

				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
		}
	}
}
