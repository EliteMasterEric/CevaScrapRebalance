using BepInEx;

using HarmonyLib;
using BepInEx.Logging;

namespace GarageDoorFix
{
    public static class PluginInfo
    {
        public const string PLUGIN_ID = "GarageDoorFix";
        public const string PLUGIN_NAME = "GarageDoorFix";
        public const string PLUGIN_VERSION = "1.0.0";
        public const string PLUGIN_GUID = "com.elitemastereric.garagedoorfix";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }

        public ManualLogSource PluginLogger;

        public PluginConfig PluginConfig;

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
