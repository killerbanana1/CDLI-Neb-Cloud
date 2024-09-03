using HarmonyLib;
using Modding;
using Munitions;
using Munitions.ModularMissiles;
using FleetEditor.MissileEditor;
using Ships;
using Bundles;
using Factions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Ships.Serialization;
using System.Linq;
using UnityEngine;
using Utility;
using UI;
using Game;
using System.Diagnostics;

[HarmonyPatch(typeof(HullComponent), "UseableByFaction")]
class Patch_HullComponent_UseableByFaction
{
    public static bool Prefix(ref HullComponent __instance, ref bool __result, ref FactionDescription faction)
    {
        // UnityEngine.Debug.Log("[Falcata] Patch_HullComponent_UseableByFaction Prefix start");
        //if (faction != null)
          //  UnityEngine.Debug.Log("\tFaction key: " + faction.SaveKey);
        //else
          //  UnityEngine.Debug.Log("\tFaction is null"); 

        // Check if faction matches
        if (faction != null && faction.SaveKey == "Falcata Republic/Falcata Republic")
        {
            UnityEngine.Debug.Log("\tSaveKey matched");
            List<string> _lockedComponents = new List<string>() { "Stock/Gun Plotting Center", "Stock/Energy Regulator", "Stock/Small Energy Regulator", "Stock/Focused Particle Accelerator" }; // Add your component keys here

            // Check if the component is in the locked list
            if (_lockedComponents.Contains(__instance.SaveKey))
            {
            //    UnityEngine.Debug.Log($"\t\tComponent \"{__instance.SaveKey}\" is forbidden");
                __result = false;
                return false; // Skip the original method
            }
        }

        // Allow original method execution for non-matching components or non-matching factions
        return true;
    }
}