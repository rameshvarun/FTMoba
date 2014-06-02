using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A class for the common logic of cooldowns.
/// Possible uses include bullet reload times, skill recharge times,
/// and creep spawn timers.
/// </summary>
[System.Serializable]
public class Cooldown {
	/// <summary>
	/// How long the cooldown takes to recharge.
	/// </summary>
	public float cooldownTime;

	/// <summary>
	/// Cooldowns can also have an 'active period'. This, for example
	/// can represent a buff that takes 6 seconds to be recharged and used again,
	/// but the buff itself is active for 3 seconds.
	/// </summary>
	public float activeTime;

	private float cooldownTimer;

	/// <summary>
	/// Provide a delagate to be called when the effect is first activated.
	/// </summary>
	public event Action onActivate = delegate {};

	/// <summary>
	/// Provide a delegate to be called when the effect switches from active to unactive.
	/// </summary>
	public event Action onDeactivate = delegate {};

	private bool isActive = false;

	public Cooldown() {
		cooldownTimer = 0.0f;
	}
	
	public Cooldown(float cooldownTime) {
		this.cooldownTime = cooldownTime;
	}

	/// <summary>
	/// This update method must be called in order to update the timers on the cooldown.
	/// </summary>
	public void Update() {
		cooldownTimer -= Time.deltaTime;

		if(isActive != Active()) {
			if(Active())
				onActivate();
			else
				onDeactivate();
			isActive = Active();
		}
	}

	/// <summary>
	/// Is the cooldown fully charged?
	/// </summary>
	/// <returns><c>true</c> if the cooldown is ready to be used; otherwise, <c>false</c>.</returns>
	public bool IsReady() {
		return cooldownTimer < 0;
	}

	/// <summary>
	/// "Use" the cooldown, forcing it to recharge.
	/// </summary>
	public void Reset() {
		cooldownTimer = cooldownTime;
	}

	/// <summary>
	/// Return if the cooldown is still within its "active" period.
	/// For example, whether or not a buff should be applied.
	/// </summary>
	public bool Active() {
		return cooldownTimer > (cooldownTime - activeTime);
	}
}