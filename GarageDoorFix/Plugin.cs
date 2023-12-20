using BepInEx;

using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Bootstrap;
using System;

namespace GarageDoorFix
{
    public static class PluginInfo
    {
        public const string PLUGIN_ID = "GarageDoorFix";
        public const string PLUGIN_NAME = "GarageDoorFix";
        public const string PLUGIN_VERSION = "1.0.0";
        public const string PLUGIN_GUID = "com.elitemastereric.GarageDoorFix";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("LC_API", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        public ManualLogSource PluginLogger;
        
        private void Awake()
        {
            Instance = this;

            PluginLogger = Logger;

            // Apply Harmony patches (if any exist)
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Plugin startup logic
            PluginLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_GUID}) is loaded!");
        }
    }
}
