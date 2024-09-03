using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ships
{
    public class PoweredDCLockerComponent : DCLockerComponent
    {
        [ShipStat("powerplant-prodefficiency", "Powerplant Efficiency", "%", InitialValue = 1f)]
        protected StatValue _statProdEfficiency;

        protected override float GetProductionEfficiency() => this._statProdEfficiency.Modifier;
    }
}
