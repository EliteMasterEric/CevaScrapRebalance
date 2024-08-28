using HarmonyLib;

namespace CevaScrapRebalance.Patch {

	[HarmonyPatch(typeof(LungProp))]
	[HarmonyPatch("DisconnectFromMachinery")]
	class LungPropDisconnectFromMachineryPatch {
		public static void Postfix(LungProp __instance) {
			Plugin.Instance.PluginLogger.LogInfo("Apparatus retrieved, fixing properties...");

			SetApparatusScrapValue(__instance);
			SetApparatusWeight(__instance);
			ScrapValueCalculator.ApplyApparatusTwoHanded(__instance.itemProperties);
			ScrapValueCalculator.ApplyApparatusConductive(__instance.itemProperties);
		}

		internal static void SetApparatusScrapValue(LungProp __instance) {
			var value = ScrapValueCalculator.CalculateApparatusScrapValue();
			Plugin.Instance.PluginLogger.LogInfo("Setting Apparatus scrap value to " + value);
			__instance.SetScrapValue(value);
		}

		internal static void SetApparatusWeight(LungProp __instance) {
			float weight = ScrapValueCalculator.CalculateApparatusScrapWeight();
			Plugin.Instance.PluginLogger.LogInfo("Setting Apparatus weight to " + weight);
			__instance.itemProperties.weight = weight;
		}
	}

	[HarmonyPatch(typeof(LungProp))]
	[HarmonyPatch("Start")]
	class LungPropStartPatch {
		public static void Postfix(LungProp __instance) {
			Plugin.Instance.PluginLogger.LogInfo("Apparatus spawned, fixing properties...");

			LungPropDisconnectFromMachineryPatch.SetApparatusScrapValue(__instance);
			LungPropDisconnectFromMachineryPatch.SetApparatusWeight(__instance);
			ScrapValueCalculator.ApplyApparatusTwoHanded(__instance.itemProperties);
			ScrapValueCalculator.ApplyApparatusConductive(__instance.itemProperties);
		}
	}
}