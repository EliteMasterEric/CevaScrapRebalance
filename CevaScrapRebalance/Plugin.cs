﻿using BepInEx;

using HarmonyLib;
using BepInEx.Logging;
using static BepInEx.BepInDependency;

namespace CevaScrapRebalance
{
    public static class PluginInfo
    {
        public const string PLUGIN_ID = "cevascraprebalance";
        public const string PLUGIN_NAME = "CevaScrapRebalance";
        public const string PLUGIN_VERSION = "1.0.1";
        public const string PLUGIN_GUID = "com.elitemastereric.cevascraprebalance";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(StaticNetcodeLib.MyPluginInfo.PLUGIN_GUID, DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        public ManualLogSource PluginLogger;
        
        internal PluginConfig PluginConfig;

        private void Awake()
        {
            Instance = this;

            PluginLogger = Logger;

            // Apply Harmony patches (if any exist)
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Plugin startup logic
            PluginLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_GUID}) is loaded!");

            LoadConfig();
        }

        private void LoadConfig()
        {
            PluginConfig = new PluginConfig();
            PluginConfig.BindConfig(Config);
        }
    }
}
