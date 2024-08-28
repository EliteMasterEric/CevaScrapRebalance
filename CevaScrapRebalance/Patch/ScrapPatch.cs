using HarmonyLib;

namespace CevaScrapRebalance.Patch {

	[HarmonyPatch(typeof(StartOfRound))]
	[HarmonyPatch("Start")]
	class StartOfRoundStartPatch {
		public static void Postfix(StartOfRound __instance) {
			Plugin.Instance.PluginLogger.LogInfo("Game initialized, applying custom scrap values...");

            ApplyForLevels(__instance.levels);
		}

        static void ApplyForLevels(SelectableLevel[] levels) {
            foreach (SelectableLevel level in levels) {
				foreach (SpawnableItemWithRarity item in level.spawnableScrap) {
					ScrapValueCalculator.ApplyForItem(level.sceneName, item);
				}
            }
        }
	}
}