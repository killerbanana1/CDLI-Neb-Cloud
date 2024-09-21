using HarmonyLib;
using Modding;
using Munitions;
using UnityEngine;
using UnityEngine.VFX;
using System;
using System.Reflection;
using System.Collections.Generic;
using Ships;
using Bundles;
using Utility;

public class CopyShellData : IModEntryPoint
{
    public void PreLoad()
    {
        // empty
    }

    public void PostLoad()
    {
        updateAllTurrets();
        updateAllMunitions();

        Harmony harmony = new Harmony("nebulous.CDLI");
        harmony.PatchAll();
        UnityEngine.Debug.Log("[CDLI] PostLoad Harmony call complete");
    }

    public static void updateAllTurrets()
    {
        Dictionary<string, HullComponent> componentDictionary = (Dictionary<string, HullComponent>)GetPrivateField(BundleManager.Instance, "_components");
        foreach (string componentName in componentDictionary.Keys)
        {
            Debug.Log(componentName);
        }
        updateTurret(componentDictionary, "Stock/Mk81 Railgun", "CDLI/PR600 'Trebuchet' Railgun");
        updateTurret(componentDictionary, "Stock/Mk81 Railgun", "CDLI/PR400 'Trebuchet' Railgun");
        updateTurret(componentDictionary, "Stock/Mk81 Railgun", "CDLI/PR200 'Trebuchet' Railgun");
    }

    public static void updateTurret(Dictionary<string, HullComponent> componentDictionary, string keySource, string keyDestination)
    {
        Debug.Log($"Called updateTurret on componentDictionary: {componentDictionary} keySource: {keySource} keyDestination: {keyDestination}");

        HullComponent componentSource, componentDestination;
        componentDictionary.TryGetValue(keySource, out componentSource);
        componentDictionary.TryGetValue(keyDestination, out componentDestination);

        Debug.Log($"componentSource: {componentSource}");
        Debug.Log($"componentDestination: {componentDestination}");

        DynamicVisibleParticles sourceDVP = (DynamicVisibleParticles)GetPrivateField(componentSource, "_disabledParticles");
        DynamicVisibleParticles destinationDVP = (DynamicVisibleParticles)GetPrivateField(componentDestination, "_disabledParticles");

        Debug.Log($"sourceDVP: {sourceDVP}");
        Debug.Log($"destinationDVP: {destinationDVP}");

        VisualEffect sourceVisualEffect = (VisualEffect)GetPrivateField(sourceDVP, "_particles");
        VisualEffect destinationVisualEffect = (VisualEffect)GetPrivateField(destinationDVP, "_particles");

        Debug.Log($"sourceVisualEffect: {sourceVisualEffect}");
        Debug.Log($"destinationVisualEffect: {destinationVisualEffect}");

        destinationVisualEffect.visualEffectAsset = sourceVisualEffect.visualEffectAsset;

        Muzzle sourceMuzzle = ((Muzzle[])GetPrivateField(componentSource, "_muzzles"))[0];
        Muzzle[] destinationMuzzles = (Muzzle[])GetPrivateField(componentDestination, "_muzzles");

        Debug.Log($"sourceMuzzle: {sourceMuzzle}");
        Debug.Log($"destinationMuzzles: {destinationMuzzles}");

        VisualEffect sourceFlash = (VisualEffect)GetPrivateField((RezzingMuzzle)sourceMuzzle, "_flash");

        foreach (Muzzle destinationMuzzle in destinationMuzzles)
        {
            VisualEffect destinationFlash = (VisualEffect)GetPrivateField((RezzingMuzzle)destinationMuzzle, "_flash");
            destinationFlash.visualEffectAsset = sourceFlash.visualEffectAsset;
        }
    }

    public static void updateAllMunitions()
    {
        Dictionary<string, IMunition> munitionDictionary = (Dictionary<string, IMunition>)GetPrivateField(BundleManager.Instance, "_munitionsBySaveKey");
        foreach (string munitionName in munitionDictionary.Keys)
        {
            Debug.Log(munitionName);
        }
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/300mm AP Rail Sabot", "CDLI/600mm APDS Shell");
        updateMunition<LightweightExplosiveShell>(munitionDictionary, "Stock/450mm HE Shell", "CDLI/600mm Shredder Shell");
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/300mm AP Rail Sabot", "CDLI/400mm APDS Shell");
        updateMunition<LightweightExplosiveShell>(munitionDictionary, "Stock/450mm HE Shell", "CDLI/400mm Shredder Shell");
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/300mm AP Rail Sabot", "CDLI/200mm APDS Shell");
        updateMunition<LightweightExplosiveShell>(munitionDictionary, "Stock/450mm HE Shell", "CDLI/200mm Shredder Shell");
    }

    public static void updateMunition<T>(Dictionary<string, IMunition> munitionDictionary, string keySource, string keyDestination)
    {
        IMunition munitionSource, munitionDestination;
        munitionDictionary.TryGetValue(keySource, out munitionSource);
        munitionDictionary.TryGetValue(keyDestination, out munitionDestination);

        T typedSource, typedDestination;
        typedSource = (T)munitionSource;
        typedDestination = (T)munitionDestination;

        SetPrivateField(typedDestination, "_effectsGroups", GetPrivateField(typedSource, "_effectsGroups"));

        object sveSource = GetPrivateField(typedSource, "_tracerEffect");
        object sveDestination = GetPrivateField(typedDestination, "_tracerEffect");

        SetPrivateField(sveDestination, "_effectAsset", (VisualEffectAsset)GetPrivateField(sveSource, "_effectAsset"));
        SetPrivateField(typedDestination, "_tracerEffect", sveDestination);
    }

    public static object GetPrivateField(object instance, string fieldName)
    {
        static object GetPrivateFieldInternal(object instance, string fieldName, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                return field.GetValue(instance);
            }
            else if (type.BaseType != null)
            {
                return GetPrivateFieldInternal(instance, fieldName, type.BaseType);
            }
            else
            {
                return null;
            }
        }

        return GetPrivateFieldInternal(instance, fieldName, instance.GetType());
    }

    public static void SetPrivateField(object instance, string fieldName, object value)
    {
        static void SetPrivateFieldInternal(object instance, string fieldName, object value, Type type)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }
            else if (type.BaseType != null)
            {
                SetPrivateFieldInternal(instance, fieldName, value, type.BaseType);
                return;
            }
        }

        SetPrivateFieldInternal(instance, fieldName, value, instance.GetType());
    }
}