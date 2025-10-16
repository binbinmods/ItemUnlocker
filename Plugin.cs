// These are your imports, mostly you'll be needing these 5 for every plugin. Some will need more.

using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using static Obeliskial_Essentials.Essentials;
using static Obeliskial_Essentials.CardDescriptionNew;
using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


// The Plugin csharp file is used to specify some general info about your plugin. and set up things for 


// Make sure all your files have the same namespace and this namespace matches the RootNamespace in the .csproj file
// All files that are in the same namespace are compiled together and can "see" each other more easily.

namespace ItemUnlocker
{
    // These are used to create the actual plugin. If you don't need Obeliskial Essentials for your mod, 
    // delete the BepInDependency and the associated code "RegisterMod()" below.

    // If you have other dependencies, such as obeliskial content, make sure to include them here.
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.stiffmeds.obeliskialessentials", BepInDependency.DependencyFlags.SoftDependency)] // this is the name of the .dll in the !libs folder.
    [BepInProcess("AcrossTheObelisk.exe")] //Don't change this

    // If PluginInfo isn't working, you are either:
    // 1. Using BepInEx v6
    // 2. Have an issue with your csproj file (not loading the analyzer or BepInEx appropriately)
    // 3. You have an issue with your solution file (not referencing the correct csproj file)


    public class Plugin : BaseUnityPlugin
    {

        // If desired, you can create configs for users by creating a ConfigEntry object here, 
        // Configs allows users to specify certain things about the mod. 
        // The most common would be a flag to enable/disable portions of the mod or the entire mod.

        // You can use: config = Config.Bind() to set the title, default value, and description of the config.
        // It automatically creates the appropriate configs.
        public static bool EssentialsInstalled = false;
        public static ConfigEntry<bool> EnableMod { get; set; }
        public static ConfigEntry<bool> EnableDebugging { get; set; }
        public static ConfigEntry<bool> UnlockAsYouGo { get; set; }
        public static ConfigEntry<bool> NoDropOnlyItems { get; set; }
        public static ConfigEntry<bool> DisableStarterItems { get; set; }
        public static ConfigEntry<bool> DisableStarterItemUpgrades { get; set; }

        public static string PluginName;
        public static string PluginGUID;
        public static string PluginVersion;

        internal static int ModDate = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource Log;

        public static string debugBase = $"{PluginInfo.PLUGIN_GUID} ";

        private void Awake()
        {

            // The Logger will allow you to print things to the LogOutput (found in the BepInEx directory)
            Log = Logger;
            // Log.LogInfo($"{PluginInfo.PLUGIN_GUID} {PluginInfo.PLUGIN_VERSION} has loaded!");

            // Sets the title, default values, and descriptions
            string modName = "Find Drop Only Items";
            EnableMod = Config.Bind(new ConfigDefinition(modName, "EnableMod"), true, new ConfigDescription("Enables the mod. If false, the mod will not work then next time you load the game."));
            EnableDebugging = Config.Bind(new ConfigDefinition(modName, "EnableDebugging"), false, new ConfigDescription("Enables the debugging"));
            UnlockAsYouGo = Config.Bind(new ConfigDefinition(modName, "UnlockAsYouGo"), true, new ConfigDescription("Whenever you see a drop-only item, it will be set to not drop-only, so you can see it in future shops."));
            NoDropOnlyItems = Config.Bind(new ConfigDefinition(modName, "NoDropOnlyItems"), false, new ConfigDescription("If true, all drop-only items will be set to not drop-only when the game launches. Requires restart"));
            DisableStarterItems = Config.Bind(new ConfigDefinition(modName, "DisableStarterItems"), false, new ConfigDescription("If true, all versions of starter items will not be included in the pool."));
            DisableStarterItemUpgrades = Config.Bind(new ConfigDefinition(modName, "DisableStarterItemUpgrades"), true, new ConfigDescription("If true, the upgraded versions of starter items will not be included in the pool."));
            // apply patches, this functionally runs all the code for Harmony, running your mod

            PluginName = PluginInfo.PLUGIN_NAME;
            PluginGUID = PluginInfo.PLUGIN_GUID;
            PluginVersion = PluginInfo.PLUGIN_VERSION;

            if (EnableMod.Value)
            {
                if (EssentialsCompatibility.Enabled)
                    EssentialsCompatibility.EssentialsRegister();
                else
                    LogInfo($"{PluginGUID} {PluginVersion} has loaded!");
                harmony.PatchAll();
            }
        }


        // These are some functions to make debugging a tiny bit easier.
        internal static void LogDebug(string msg)
        {
            if (EnableDebugging.Value)
            {
                Log.LogDebug(debugBase + msg);
            }

        }
        internal static void LogInfo(string msg)
        {
            Log.LogInfo(debugBase + msg);
        }
        internal static void LogError(string msg)
        {
            Log.LogError(debugBase + msg);
        }


    }
}