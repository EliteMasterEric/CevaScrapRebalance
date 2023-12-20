using HarmonyLib;
using UnityEngine;

namespace GarageDoorFix.Patch {

    [HarmonyPatch(typeof(InteractTrigger))]
    [HarmonyPatch("Start")]
    class InteractTriggerStartPatch {
        const string GARAGE_TRIGGER_SCENE = "Level1Experimentation";
        const string GARAGE_TRIGGER_NAME = "Cube";
        const string GARAGE_TRIGGER_PARENT_NAME = "Cutscenes";

        public static void Postfix(InteractTrigger __instance) {
            try {
                Plugin.Instance.PluginLogger.LogDebug("Interact Trigger spawned, querying...");

                GameObject triggerGameObject = __instance.gameObject;

                if (triggerGameObject.scene.name == GARAGE_TRIGGER_SCENE) {
                    Plugin.Instance.PluginLogger.LogDebug("Current stage is Experimentation, querying...");
                    
                    if (triggerGameObject.name == GARAGE_TRIGGER_NAME && triggerGameObject.transform.parent.name == GARAGE_TRIGGER_PARENT_NAME) {
                        Plugin.Instance.PluginLogger.LogInfo("Found garage trigger! Modifying...");
                        __instance.triggerOnce = Plugin.Instance.PluginConfig.ShouldDoorDropOnlyOnce();
                        __instance.randomChancePercentage = Plugin.Instance.PluginConfig.GetGarageDoorChance();
                    }
                }
            }
            catch (System.Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in InteractTriggerStartPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }
    }
}