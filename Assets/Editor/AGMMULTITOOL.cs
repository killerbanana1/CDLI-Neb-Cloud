using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System;
using System.Linq;
//using FleetEditor.MissileEditor;
//using Munitions.ModularMissiles;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using System.Reflection;
//using Ships;
//using static Sound.AnnouncerVoiceSet;
using UnityEngine.UI;
//using static UnityEngine.Rendering.DebugUI;
//using UnityEngine.InputSystem;
//using UnityEngine.UIElements;
//using log4net.Util;
//using Steamworks;
//using SOHolder;



[InitializeOnLoad]
public class Information : EditorWindow
{
    static string[] bundles = { "stock", "stock-f1", "stock-f2", "stock-maps" };
    Dictionary<string, bool> foldouts = new Dictionary<string, bool>();
    string mods = " muzzle-accuracy, sensor-maxrange, sensor-power, sensor-gain, sensor-sensitivity, sensor-aperture, sensor-maxposerror, sensor-maxvelerror, sensor-lockmultiplier, sensor-minlocksnr, sensor-noisefiltering, sensor-burnthrupowermultiplier, sensor-burnthrudamageprob, component-maxhp, component-dr, component-workspeed, component-raredebuffchance, intel-effort, intel-accuracy, comms-power, comms-bandwidth, comms-gain, continuousweapon-burstduration, continuousweapon-cooldown, continuousweapon-overheatdamageprob, continuousweapon-directdamagemultiplier, damagecontrol-movespeed, damagecontrol-repairspeed, discreteweapon-recycle, discreteweapon-reload, powerplant-prodefficiency, hull-mass, hull-maxspeed, hull-turnrate, hull-linearmotor, hull-angularmotor, hull-sigmult-radar, hull-crewvuln, hull-armorthickness, hull-componentdr, hull-visiondistance, hull-flankdamageprob, hull-identificationwork, hull-missilechannels, hull-missileprogspeed, hull-structurehp, tubelauncher-reload, turret-traverse, turret-elevate, missile-health, missile-progtime, missile-sigmult-radar, missile-boostphaseduration, missile-earlystrafe, missile-earlyturn, missile-failrate";
    int tab;
    bool dimensions, tips, texturing, modulesizes = true;
    bool sprinter, raines, keystone, vauxhall, axford, solomon = false;
    string warshipdims =

    "1 unit in Unity is 10 meters (m) to scale, L x W x H\n\n" +

    "Sprinter-class\n" +
    "80x37x28m\n\n" +

    "Raines-class\n" +
    "96x35x40m\n\n" +

    "Keystone-class\n" +
    "130x50x45m\n\n" +

    "Vauxhall-class\n" +
    "215x62x60m\n\n" +

    "Axford-class\n" +
    "195x70x75m\n\n" +

    "Solomon-class\n" +
    "285x80x105m\n";


    string muntionkeys =
    "Stock/250mm HE Shell\n" +
    "Stock/250mm AP Shell\n" +
    "Stock/250mm HE-RPF Shell\n" +
    "Stock/450mm HE Shell\n" +
    "Stock/450mm AP Shell\n" +
    "Stock/20mm Slug\n" +
    "Stock/Flak Round\n" +
    "Stock/120mm HE Shell\n" +
    "Stock/120mm AP Shell\n" +
    "Stock/120mm HE-RPF Shell\n" +
    "Stock/15mm Sandshot\n" +
    "Stock/300mm AP Rail Sabot\n" +
    "Stock/EA12 Chaff Decoy\n" +
    "Stock/EA20 Flare Decoy\n" +
    "Stock/EA99 Active Decoy\n" +
    "Stock/100mm HE Shell\n" +
    "Stock/100mm HE-HC Shell\n" +
    "Stock/100mm AP Shell\n" +
    "Stock/100mm Grapeshot\n" +
    "Stock/400mm Plasma Ampoule\n" +
    "Stock/500mm Fracturing Block\n" +
    "Stock/S1 Rocket\n" +
    "Stock/S3 Mine\n" +
    "Stock/S3 Net Mine\n" +
    "Stock/S3 Sprint Mine\n" +
    "Stock/Mine Container\n" +
    "Stock/Rocket Container\n" +
    "Stock/Decoy Container(Clipper)\n" +
    "Stock/Decoy Container(Line Ship)\n";



    static Information()
    {
        return;
    }

    [MenuItem("AGM's Toolkit/Informantion")]
    static void ShowWindow()
    {

        EditorWindow window = EditorWindow.GetWindow(typeof(Information));
        window.Show();
    }
    public static void DrawSelectableLabel(string text, GUIStyle style = null)
    {
        if (style == null)
            style = GUI.skin.label;

        var content = new GUIContent(text);
        var position = GUILayoutUtility.GetRect(content, style);
        EditorGUI.SelectableLabel(position, text, style);
    }

    void OnGUI()
    {
        tab = GUILayout.Toolbar(tab, new string[] { "Modifers", "Dimensions", "Tips", "Munition Keys" });
        switch (tab)
        {
            case 0:
                string[] modlist;
                List<string> categorylist = new List<string>();
                List<string> shortcategorylist = new List<string>();
                //Vector2 scrollPosition;
                string pmods;
                pmods = mods.Replace(",", "");
                modlist = pmods.Split(' ');
                foreach (string mod in modlist)
                    categorylist.Add(mod.Split('-')[0]);
                shortcategorylist = categorylist.Distinct().ToList();

                foreach (string category in shortcategorylist)
                    if (!foldouts.ContainsKey(category)) foldouts.Add(category, false);

                GUIStyle mystyle = new GUIStyle("WhiteLabel");
                foreach (string category in shortcategorylist)
                {
                    if (category == "")
                        continue;
                    foldouts[category] = EditorGUILayout.Foldout(foldouts[category], category);
                    if (foldouts[category])
                        for (int i = 0; i < modlist.Length; i++)
                            if (category == categorylist[i])
                                DrawSelectableLabel(modlist[i]);
                }
                break;
            case 1:
                dimensions = EditorGUILayout.Foldout(dimensions, "Ship Dimensions");
                if (dimensions)
                    DrawSelectableLabel(warshipdims);
                modulesizes = EditorGUILayout.Foldout(modulesizes, "Module Sizes");
                if (modulesizes)
                    DrawSelectableLabel("C1 is 2x2 = 5x5m\nC2 is 3x3 = 7.5x7.5m\nC3 is 3x5 = 7.5x12.5m\nSpinal is 4x12x4 = 10m x 30m x 10m\nC4 is 6x6 = 15x15m\nC5 is 8x8 = 20x20m\n");
                sprinter = EditorGUILayout.Foldout(sprinter, "Sprinter Internals");
                if (sprinter)
                    DrawSelectableLabel("Sprinter-class Corvette\r\n58x30x25m\r\n2 C1\r\n1 C2\r\n1 C3 \r\nCompartments:\r\n2 4x1x6 \r\n2 3x1x3\r\nModules:\r\n1 3x3x3\r\n1 8x3x6\r\n2 2x2x2");
                raines = EditorGUILayout.Foldout(raines, "Raines Internals");
                if (raines)
                    DrawSelectableLabel("Raines-class Frigate\r\n96x35x40m\r\n4 C2\r\nCompartments:\r\n2 4x1x6\r\n1 3x1x5\r\n2 3x1x3\r\n1 8x3x6\r\nModules:\r\n1 6x6x6\r\n3 2x2x2");
                keystone = EditorGUILayout.Foldout(keystone, "Keystone Internals");
                if (keystone)
                    DrawSelectableLabel("Keystone-class Destroyer\r\n130x50x45m\r\n2 C1\r\n3 C2\r\n2 C3\r\n1 Spinal\r\nCompartments:\r\n2 6x1x8\r\n5 6x1x4\r\nModules:\r\n1 8x8x6\r\n1 6x6x6\r\n3 3x3x3\r\n2 2x2x2");
                vauxhall = EditorGUILayout.Foldout(vauxhall, "Vauxhall Internals");
                if (vauxhall)
                    DrawSelectableLabel("Vauxhall-class Light Cruiser\r\n215x62x60m\r\n4 C1\r\n6 C3\r\nCompartments:\r\n5 6x1x8\r\n3 6x1x4\r\n1 4x1x8\r\nModules:\r\n1 8x12x10\r\n1 6x6x6\r\n3 3x3x3\r\n4 2x2x2");
                axford = EditorGUILayout.Foldout(axford, "Axford Internals");
                if (axford)
                    DrawSelectableLabel("Axford-class Heavy Cruiser\r\n195x70x75m\r\n4 C1\r\n4 C2\r\n2 C4\r\n1 C5\r\nCompartments:\r\n5 6x1x8\r\n2 6x1x6\r\n4 6x1x4\r\nModules:\r\n1 12x8x10\r\n2 6x6x6\r\n4 3x3x3\r\n5 2x2x2");
                solomon = EditorGUILayout.Foldout(solomon, "Solomon Internals");
                if (solomon)
                    DrawSelectableLabel("Solomon-class Battleship\r\n285x80x105m\r\n4 C1 \r\n6 C2\r\n1 C4\r\n3 C5\r\nCompartments:\r\n9 6x1x8\r\n7 6x1x4\r\nModules:\r\n1 16x8x10\r\n1 12x8x10\r\n2 8x8x8\r\n7 3x3x3\r\n4 2x2x2 \r\n");
                break;
            case 2:
                tips = EditorGUILayout.Foldout(tips, "Unity");
                if (tips)
                    DrawSelectableLabel("Always remember that +Z is forward in game engines, which is not the case in 3d modeling packages\n");
                
                texturing = EditorGUILayout.Foldout(texturing, "Texturing");
                if (texturing)
                    DrawSelectableLabel("ffc3c3c3 for the primary paint\r\nffe6e6e6 for the stripes\n");

                break;
            case 3:
                DrawSelectableLabel(muntionkeys);
                break;
            default:
                break;
        }

    }

}


[InitializeOnLoad]
public class AGMMULTITOOL : EditorWindow
{
    static string[] bundles = { "stock", "stock-f1", "stock-f2", "stock-maps" };
    static AssetBundle stock;
    string abloc = "";


    string path = "C:/Steam/steamapps/common/Nebulous";
    bool groupEnabled;

    [MenuItem("Load Asset/Load All Assets")]
    static void LoadAsset()
    {
        bool warning = EditorUtility.DisplayDialog("WARNING",
        "This may cause performance issues and will prevent assets from being individually loaded"
        , "Load Assets", "Do not load assets");
        if (warning)
        {
            foreach (string bundle in bundles)
            {
                var stock = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath + "/Editor/AssetBundles/" + bundle));
                var prefabs = stock.LoadAllAssets<GameObject>();

                foreach (GameObject prefab in prefabs)
                {
                    Instantiate(prefab);
                }
            }

        }


    }


    [MenuItem("AGM's Toolkit/Quick Build Uncompressed AssetBundles")]
    static void BuildUncompressedAssetBundle()
    {
        CreateFolders();
        string assetBundleDirectory = "Assets/Editor/AssetBundles";
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("AGM's Toolkit/Build LZ4 Compressed AssetBundles (Recommended)")]
    static void BuildChunkBasedCompression()
    {
        CreateFolders();
        string assetBundleDirectory = "Assets/Editor/AssetBundles";
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    [MenuItem("AGM's Toolkit/Build LZMA Compressed AssetBundles")]
    static void BuildNone()
    {
        CreateFolders();
        string assetBundleDirectory = "Assets/Editor/AssetBundles";
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }


    /*
    static void BuildAllAssetBundles()
    {
        CreateFolders();
        string assetBundleDirectory = "Assets/Editor/AssetBundles";
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        string[] filePaths = Directory.GetFiles(Application.dataPath + "/Editor/AssetBundles/");
        //Debug.Log(Application.dataPath + "/Editor/AssetBundles/");
        foreach (var filename in filePaths)
        {
            string file = filename.ToString();
            //Debug.Log(file);
            //Do your job with "file"  
            string str = Application.dataPath + "/Editor/Mods/" + file.ToString();
            if (true)// !File.Exists(str)
            {
                //File.Copy(file, str);
            }
        }
    }
    */


    [MenuItem("AGM's Toolkit/Setup")]
    static void ShowWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(AGMMULTITOOL));
        window.Show();
    }


    [MenuItem("AGM's Toolkit/Generate Advanced Load Asset Menu")]
    static void LoadAdvancedAssetMenu()
    {
        string[] dlls = Directory.GetFiles(Application.dataPath, "*.dll", SearchOption.AllDirectories);
        foreach(string dll in dlls)
        {
            //Debug.Log("DLL NAME: " + dll);
            if (dll.Contains("Nebulous"))
            {
                LoadAssetMenu(true);
                return;
            }
        }
        LoadAssetMenu(false);

    }

    [MenuItem("AGM's Toolkit/Generate Load Asset Menu")]
    static void LoadBasicAssetMenu()
    {
        LoadAssetMenu(false);
    }

    static void LoadAssetMenu(bool advanced = false)
    {
        Debug.Log("Asset menu advanced: " + advanced);
        CreateFolders();
        string scriptFile = Application.dataPath + "/Editor/GeneratedCode/GeneratedMenuItems.cs";
        File.AppendAllText(scriptFile, "\n");

        // The class string
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// This class is Auto-Generated");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEditor;");
        sb.AppendLine("using System.IO;");
        sb.AppendLine("using System.Reflection;");
        if (advanced)
        {
            sb.AppendLine("using Munitions;");
            sb.AppendLine("using Munitions.ModularMissiles;");
            sb.AppendLine("using FleetEditor.MissileEditor;");


        }
        sb.AppendLine("");
        sb.AppendLine("public class Holder : MonoBehaviour");
        sb.AppendLine("{");
        sb.AppendLine("    public ScriptableObject a;");
        sb.AppendLine("}");
        sb.AppendLine("public class GeneratedMenuItems : MonoBehaviour {");


        //string[] bundles = { "stock", "statesurplus", "ryleksraiders" };
        foreach( string bundle in bundles)
        {
            string fixedBundle = bundle.Replace("-", null);
            sb.AppendLine("    static AssetBundle " + fixedBundle + "; ");
        }

        sb.AppendLine(@"
        public static object GetPrivateField(object instance, string fieldName)
        {
            static object GetPrivateFieldInternal(object instance, string fieldName, System.Type type)
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
        ");

        sb.AppendLine("    ");
        int i = 0;
        foreach (string bundle in bundles)
        {
            string fixedBundle = bundle.Replace("-", null);
            Debug.Log(bundle);
            if (!System.IO.File.Exists(Application.dataPath + "/Editor/AssetBundles/" + bundle + ".manifest"))
                continue;
            
            string fullstring = "";
            foreach (string line in System.IO.File.ReadLines(Application.dataPath + "/Editor/AssetBundles/" + bundle + ".manifest"))
            {
                if (line.Contains(":"))
                    continue;
                if (line[0] == '-')
                    fullstring = line;
                else
                    fullstring += line.Substring(1);


                if (line.Contains(".prefab")  || line.Contains(".asset"))
                {
                    //Debug.Log(line);
                    string shortline = fullstring.Substring(18);
                    if(bundle == "ryleksraiders")
                        shortline = fullstring.Substring(15);
                    string assetname = Path.GetFileNameWithoutExtension(shortline);
                    if (!bundle.StartsWith("stock"))
                    {
                        sb.AppendLine("    [MenuItem(\"Load Asset/" + bundle + "/" + shortline + "\")]");
                    }
                    else 
                    {
                        sb.AppendLine("    [MenuItem(\"Load Asset/" + shortline + "\")]");
                    }
                    
                    sb.AppendLine("    private static void MenuItem" + i.ToString() + "() {");
                    sb.AppendLine("        Debug.Log(\"Selected item: " + assetname + "\");");
                    sb.AppendLine("        if (" + fixedBundle + " == null)");
                    sb.AppendLine("            " + fixedBundle + " = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath + \"/Editor/AssetBundles/" + bundle + "\"));");
                    if (line.Contains(".prefab"))
                    {
                        sb.AppendLine("        GameObject goh = Instantiate(" + fixedBundle + ".LoadAsset<GameObject>(\"" + assetname + "\"));");
                        if (advanced)
                        {
                            sb.AppendLine("        ModularMissile missile = goh.GetComponent<ModularMissile>();");
                            sb.AppendLine("        if (missile != null)");
                            sb.AppendLine("            Instantiate(((MissileSchematic)GetPrivateField(missile, \"_schematicPrefab\")).gameObject);");
                        }

                    }
                    if (line.Contains(".asset"))
                    {
                        sb.AppendLine("        GameObject goh = new GameObject(\"" + assetname + " holder" + "\");");
                        sb.AppendLine("        goh.AddComponent<Holder>().a = " + fixedBundle + ".LoadAsset<ScriptableObject>(\"" + assetname + "\"); ");
                    }
                    sb.AppendLine("        ");
                    sb.AppendLine("    }");
                    sb.AppendLine("");
                    i++;
                }

            }
        }


        sb.AppendLine("");
        sb.AppendLine("}");

        // writes the class and imports it so it is visible in the Project window
        System.IO.File.Delete(scriptFile);
        System.IO.File.WriteAllText(scriptFile, sb.ToString(), System.Text.Encoding.UTF8);
        AssetDatabase.ImportAsset("Assets/Editor/GeneratedCode/GeneratedMenuItems.cs");
    }


    void OnGUI()
    {
        CreateFolders();
        GUILayout.Label("Asset Loading", EditorStyles.boldLabel);
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        if (GUILayout.Button("Set Current Nebulous Directory"))
        {
            //Debug.Log("Clicked the image");
            path = EditorUtility.OpenFolderPanel("Set Neb Directory", path, "");
        }
   
        path = EditorGUILayout.TextField("Nebulous Install", path);
        abloc = path + "/Assets/AssetBundles/stock";
        if (File.Exists(abloc))
            groupEnabled = true;
        else
            groupEnabled = false;
        groupEnabled = EditorGUILayout.BeginToggleGroup("Install Valid", groupEnabled);
        if (GUILayout.Button("Grab Assetbundle"))
        {
            foreach (string bundle in bundles)
            {
                string bundlePath = path + "/Assets/AssetBundles/" + bundle;
                string targetPath = Application.dataPath + "/Editor/AssetBundles/" + bundle;
                FileUtil.ReplaceFile(bundlePath, targetPath);
                FileUtil.ReplaceFile(bundlePath + ".manifest", targetPath + ".manifest");
            }

        }
        if (GUILayout.Button("Grab DLLS"))
        {
            bool warning = EditorUtility.DisplayDialog("WARNING",
            "If this project has dlls already and they aren't in Assets/LIB running this will break the project" 
            , "Copy", "Do not Copy");
            if (warning)
            {
                if (Directory.GetFiles(Application.dataPath, ".dll", SearchOption.AllDirectories).Length != 0)
                {
                    warning = EditorUtility.DisplayDialog("WARNING DLLs Detected",
                    "This project has dlls already and if they aren't in Assets/LIB running copying these dlls can cause damage"
                    , "Copy (Danger)", "Do not Copy");
                }
            }
            
            if(warning)
            {
                Directory.CreateDirectory(Application.dataPath + "/Lib");
                string[] dlls = {
                "Facepunch.Steamworks.Win64","kcp2k","Mirror", "Mirror.Components","Nebulous","Priority Queue","QFSW.QC","QuickGraph.All","QuickGraph.Core","QuickGraph.Serialization","RSG.Promise","ShapesRuntime","Telepathy","UIExtensions","Unity.Addressables","Unity.ResourceManager","Vectrosity","where-allocations", "Surge","XNode", "Unity.Burst", "MoreLinq", "Unity.Burst.Unsafe"};
                foreach (string dllname in dlls)
                {
                    FileUtil.ReplaceFile(path + "\\Nebulous_Data\\Managed\\" + dllname + ".dll", Application.dataPath + "\\Lib\\" + dllname + ".dll");
                }
            }



        }
        EditorGUILayout.EndToggleGroup();
    }
    static void CreateFolders()
    {
        Directory.CreateDirectory(Application.dataPath + "/Lib");
        Directory.CreateDirectory(Application.dataPath + "/Editor");
        //Directory.CreateDirectory(Application.dataPath + "/Editor/Plugins");
        Directory.CreateDirectory(Application.dataPath + "/Editor/GeneratedCode");
        Directory.CreateDirectory(Application.dataPath + "/Editor/AssetBundles");
        Directory.CreateDirectory(Application.dataPath + "/Editor/Mods");
    }

    void Start()
    {

        //string localPath = "Assets/" + gameObject.name + ".prefab";
        //var anotherGo = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        //PrefabUtility.SaveAsPrefabAssetAndConnect(anotherGo, localPath, InteractionMode.AutomatedAction);
    }
    void OnEnable()
    {
        CreateFolders();
        if (GraphicsSettings.currentRenderPipeline)
        {
            if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HighDefinition"))
            {
                Debug.Log("HDRP active");
                //EditorUtility.DisplayDialog("Render Pipeline Error", "You are using the Built in render pipeline when you should be using the High Definetion Render Pipeline", "Ok");
            }
            else
            {
                Debug.LogError("URP active");
                EditorUtility.DisplayDialog("Render Pipeline Error", "You are using the Universal Render Pipeline when you should be using the High Definetion Render Pipeline", "Ok");
            }
        }
        else
        {
            Debug.LogError("Built-in RP active");
            EditorUtility.DisplayDialog("Render Pipeline Error", "You are using the Built in render pipeline when you should be using the High Definetion Render Pipeline", "Ok");
        }
    }
    static AGMMULTITOOL()
    {
        return;
    }
}






