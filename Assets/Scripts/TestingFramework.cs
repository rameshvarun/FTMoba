using UnityEngine;
using System.Collections;


public class TestingFramework
{
	public static bool localServer = false;
	public static bool localClient = false;

	public static void Initialize() {
		string[] args = System.Environment.GetCommandLineArgs();
		foreach(string arg in args) {
			if(arg.ToLower() == "localserver") localServer = true;
			if(arg.ToLower() == "localclient") localClient = true;
		}
	}
}

