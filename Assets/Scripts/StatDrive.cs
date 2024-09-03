
using Ships;
using System;
using UnityEngine;

class StatDrive : DriveComponent
{
	[Header("Speed Thresholds")]
	[Tooltip("This is the first half of the speed thresholds. These are the percentage of max speed under which modifier applies. The first threshold for the first effect, and so on. There must be exactly the same amount of thresholds as modifiers.")]
	[SerializeField]
	private float[] _underSpeedThresholds;

	[Tooltip("This is the second half of the speed thresholds. These are the percentage of max speed above which modifier applies. The first threshold for the first effect, and so on. There must be exactly the same amount of thresholds as modifiers.")]
	[SerializeField]
	private float[] _aboveSpeedThresholds;

	[Tooltip("These are the effects that will be applied at the different thresholds. Ignore the duration field. The first effect for the first threshold, and so on. There must be exactly the same amount of modifiers as thresholds.")]
	[SerializeField]
	private DiscreteWeaponComponent.TemporaryFiringModifier[] _speedModifiers;

	private bool[] _didSetModifier = new bool[0];

	private bool[] DidSetModifier
	{
		get
		{
			if (_didSetModifier.Length == 0)
			{
				_didSetModifier = new bool[this._underSpeedThresholds.Length];
			}
			return _didSetModifier;
		}
	}

	public override string GetFormattedStats(bool full, int groupSize = 1)
	{
		var stats = base.GetFormattedStats(full, groupSize);

		stats += "\n<b>Speed Modifiers:</b>\n";
		for (int i = 0; i < this._speedModifiers.Length; i++)
		{
			var effect = this._speedModifiers[i];
			var threshold = this._underSpeedThresholds[i];
			var positiveBad = StatTable.GetStatInfo(effect.Modifier.StatName, out string subtype).PositiveBad;
			stats += $"{StatModifier.FormatModifierColoredLinked(effect.Modifier.StatName, effect.Modifier.Modifier, positiveBad)} when under {(int)(threshold * 100)}% Max Speed\n";
		}
		return stats;
	}

	protected override void Awake()
	{
		base.Awake();
		SetModifiers();
	}

	protected override void Update()
	{
		base.Update();
		SetModifiers();
	}

	private void SetModifiers()
	{
		var speed = this._myHull?.MyShip?.Controller?.Velocity.magnitude;
		var maxSpeed = this._myHull?.MyShip?.Controller?.MaxSpeed;
		if (speed != null && maxSpeed != null && this._speedModifiers != null && maxSpeed > 0)
		{
			for (int i = 0; i < this._speedModifiers.Length; i++)
			{
				var effect = this._speedModifiers[i];
				var upperSpeed = this._underSpeedThresholds[i] * maxSpeed;
				var lowerSpeed = this._aboveSpeedThresholds[i] * maxSpeed;
				if (speed >= lowerSpeed && speed <= upperSpeed && !this.DidSetModifier[i])
				{
					this._myHull.MyShip.AddStatModifier(this, effect.Modifier);
					this.DidSetModifier[i] = true;
				}
				else if (this.DidSetModifier[i])
				{
					this.DidSetModifier[i] = false;
				}
			}
		}
	}
}