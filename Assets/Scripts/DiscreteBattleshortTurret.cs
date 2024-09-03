using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
	// When BSHORTed the Reload goes down.
	public class DiscreteBattleshortTurret : TurretedDiscreteWeaponComponent
	{

		private bool doDamageOnFire = false;

		protected override float _cycleLength
		{
			get
			{
				if (base._battleShortEnabled)
				{
					return base._cycleLength * (1 - this._bshortReloadReductionPercent);
				}
				return base._cycleLength;
			}
		}

		protected override void RunTimers(float deltaTime)
		{
			if (base._battleShortEnabled)
			{
				// because RunTimers is going to add this back
				base._reloadAccum -= deltaTime;
				base._reloadAccum += deltaTime / (1 - this._bshortReloadReductionPercent);
				doDamageOnFire = true;
			}
			base.RunTimers(deltaTime);
		}

		public override string GetFormattedStats(bool full, int groupSize = 1)
		{
			var stats = base.GetFormattedStats(full, groupSize);

			var reloadValue = this._statReloadTime.Value * (1 - this._bshortReloadReductionPercent);
			var reloadModifier = 1 - (reloadValue / this._statReloadTime.BaseValue);
			var text = StatValue.FormatStatTextWithLink(this._statReloadTime.StatID, this._statReloadTime.DisplayName, this._statReloadTime.Unit, reloadValue, this._statReloadTime.LiteralModifier, reloadModifier * -1, this._statReloadTime.Attribute);
			stats = stats + "Battle-Short: Available\n   " + text + "\n   " + this._statOverheatDamage.FullTextWithLink + "\n";

			return stats;
		}

		protected override void OnTarget(Vector3 aimPoint, bool changed)
		{
			var reloading = this._reloading;
			base.OnTarget(aimPoint, changed);
			if (reloading != this._reloading && this.doDamageOnFire)
			{
				// Fired
				base.DoDamageToSelf(this._statOverheatDamage.Value, 1, 1);
				this.doDamageOnFire = false;
			}
		}

		[SerializeField]
		private float _bshortReloadReductionPercent = 0.5f;

		[ShipStat("discreteweapon-overheatdamage", "Overheat Damage", "hp", InitializeFrom = "_overheatDamage", MinValue = 0.01f, PositiveBad = true, StackingPenalty = true, TextValueMultiplier = 1f, NameSubtypeFrom = "_overheatDamageSubtype", LimitSubtypeModifiersOnly = true)]
		protected StatValue _statOverheatDamage;

		[SerializeField]
		private float _overheatDamage = 10f;

		[SerializeField]
		private string _overheatDamageSubtype;
	}
}
