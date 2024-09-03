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

        Harmony harmony = new Harmony("nebulous.Falcata");
        harmony.PatchAll();
        UnityEngine.Debug.Log("[Falcata] PostLoad Harmony call complete");
    }

    public static void updateAllTurrets()
    {
        Dictionary<string, HullComponent> componentDictionary = (Dictionary<string, HullComponent>)GetPrivateField(BundleManager.Instance, "_components");
        foreach (string componentName in componentDictionary.Keys)
        {
            Debug.Log(componentName);
        }
        updateTurret(componentDictionary, "Stock/Mk68 Cannon", "Falcata Republic/RHI-4380 Cannon");
        updateTurret(componentDictionary, "Stock/Mk68 Cannon", "Falcata Republic/RHI-3380 Cannon");
        updateTurret(componentDictionary, "Stock/Mk68 Cannon", "Falcata Republic/RHI-2380 Cannon");
        updateTurret(componentDictionary, "Stock/Mk65 Cannon", "Falcata Republic/RHI-4150 Cannon");
        updateTurret(componentDictionary, "Stock/Mk65 Cannon", "Falcata Republic/RHI-2150 Cannon");
        updateTurret(componentDictionary, "Stock/Mk65 Cannon", "Falcata Republic/RHI-1620 Recoilless Rifle");
        updateTurret(componentDictionary, "Stock/C65 Cannon", "Falcata Republic/RHI-S820 Recoilless Rifle");
        updateTurret(componentDictionary, "Stock/C65 Cannon", "Falcata Republic/RHI-S1700 Kinetic Artillery");
        updateTurret(componentDictionary, "Stock/C53 Cannon", "Falcata Republic/RHI-S1150 Cannon");
        updateTurret(componentDictionary, "Stock/C53 Cannon", "Falcata Republic/RHI-S4150 Cannon");
        updateTurret(componentDictionary, "Stock/Mk81 Railgun", "Falcata Republic/WY-135 Railgun");
        updateTurret(componentDictionary, "Stock/Mk81 Railgun", "Falcata Republic/WY-235 Railgun");
        updateTurret(componentDictionary, "Stock/Mk64 Cannon", "Falcata Republic/WY-1220 ETC Gun");
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
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/450mm AP Shell", "Falcata Republic/380mm AP Shell");
        updateMunition<LightweightExplosiveShell>(munitionDictionary, "Stock/450mm HE Shell", "Falcata Republic/380mm HE Shell");
        updateMunition<LightweightAirburstFragShell>(munitionDictionary, "Stock/600mm Bomb Shell", "Falcata Republic/380mm HE-ABF Shell");
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/450mm AP Shell", "Falcata Republic/200mm RP-HEI Shell");
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/450mm AP Shell", "Falcata Republic/220mm AP Shell");
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/450mm AP Shell", "Falcata Republic/700mm KART-HEI Shell");
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/450mm AP Shell", "Falcata Republic/700mm KART-KP Shell");
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/300mm AP Rail Sabot", "Falcata Republic/35mm AP Rail Pinshot");
        updateMunition<LightweightExplosiveShell>(munitionDictionary, "Stock/250mm HE Shell", "Falcata Republic/150mm HE Shell");
        updateMunition<LightweightProximityShell>(munitionDictionary, "Stock/250mm HE-RPF Shell", "Falcata Republic/150mm Flak Shell");
        updateMunition<LightweightKineticShell>(munitionDictionary, "Stock/250mm AP Shell", "Falcata Republic/150mm AP-S Shell");
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