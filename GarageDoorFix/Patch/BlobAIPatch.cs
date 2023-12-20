using HarmonyLib;

namespace GarageDoorFix.Patch {
    [HarmonyPatch(typeof(BlobAI))]
    [HarmonyPatch("OnCollideWithPlayer")]
    class BlobAIOnCollideWithPlayerPatch {
        public static bool Prefix(BlobAI __instance) {
            var tamedTimer = Traverse.Create(__instance).Field("tamedTimer").GetValue<float>();
            var angeredTimer = Traverse.Create(__instance).Field("angeredTimer").GetValue<float>();

            if (tamedTimer > 0.0f && angeredTimer <= 0.0f) {
                // Plugin.Instance.PluginLogger.LogInfo("Blob is tamed! Skipping...");
                return false; // Skip original and other patches.
            }

            return true;
        }
    }
}