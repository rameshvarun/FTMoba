using UnityEngine;
using System.Collections;

public class Utils {
	public static bool RandomChance() {
		return RandomChance(0.5);
	}

	public static bool RandomChance(double chance) {
		return Random.value > chance;
	}
}

