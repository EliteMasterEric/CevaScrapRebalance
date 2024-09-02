using System;

namespace CevaScrapRebalance
{
    internal class ScrapValueCalculator
    {
        public const double BASE_SCRAP_MULTIPLIER = 1.0 / 0.4;

        public const float WEIGHT_MULTIPLIER = 105.0f;

        public static void ApplyForItem(string sceneName, SpawnableItemWithRarity item)
        {
            Item itemProperties = item.spawnableItem;
            var scrapKey = PluginConfig.SanitizeScrapKey(itemProperties.itemName);
            if (!HasScrapConfig(scrapKey))
            {
                Plugin.Instance.PluginLogger.LogDebug("Item " + scrapKey + " has no scrap config, leaving numbers as default.");
                return;
            }

            Plugin.Instance.PluginLogger.LogDebug("Item " + scrapKey + " has a scrap config, applying custom values...");

            var minValue = CalculateScrapMinValue(scrapKey);
            if (minValue >= 0)
            {
                Plugin.Instance.PluginLogger.LogDebug("  - Min Value: " + minValue);
                itemProperties.minValue = minValue;
            }
            else
            {
                Plugin.Instance.PluginLogger.LogDebug("  - Min Value: DEFAULT");
            }

            var maxValue = CalculateScrapMaxValue(scrapKey);
            if (maxValue >= 0)
            {
                Plugin.Instance.PluginLogger.LogDebug("  - Max Value: " + maxValue);
                itemProperties.maxValue = maxValue;
            }
            else
            {
                Plugin.Instance.PluginLogger.LogDebug("  - Max Value: DEFAULT");
            }

            var displayWeight = CalculateScrapWeight(scrapKey);
            var weight = CalculateScrapInternalWeight(scrapKey);
            if (displayWeight >= 0)
            {
                Plugin.Instance.PluginLogger.LogDebug("  - Weight: " + displayWeight + " (" + weight + ")");
                itemProperties.weight = weight;
            }
            else
            {
                Plugin.Instance.PluginLogger.LogDebug("  - Weight: DEFAULT");
            }

            var rarity = CalculateScrapRarity(scrapKey, sceneName);
            if (rarity >= 0)
            {
                Plugin.Instance.PluginLogger.LogDebug("  - Rarity (" + sceneName + "): " + rarity);
                item.rarity = rarity;
            }
            else
            {
                Plugin.Instance.PluginLogger.LogDebug("  - Rarity (" + sceneName + "): DEFAULT");
            }

            ApplyItemTwoHanded(scrapKey, itemProperties);

            ApplyItemConductive(scrapKey, itemProperties);
        }

        public static int CalculateApparatusScrapValue()
        {
            ScrapConfig apparatusConfig = Plugin.Instance.PluginConfig.FetchApparatusConfig();

            var minValue = apparatusConfig.minValue.Value;
            var maxValue = apparatusConfig.maxValue.Value;

            if (minValue < 0 && maxValue < 0)
            {
                // Tell it to use the default.
                return -1;
            }

            if (minValue < 0)
            {
                minValue = 80;
            }

            if (minValue < 0)
            {
                maxValue = 80;
            }

            if (minValue == maxValue)
            {
                // Values are equal, no randomization needed.
                return minValue;
            }

            if (minValue > maxValue)
            {
                Plugin.Instance.PluginLogger.LogWarning("Apparatus min value is greater than max value. Swapping...");

                var temp = maxValue;
                minValue = maxValue;
                maxValue = temp;
            }

            var scrapValue = RoundManager.Instance.AnomalyRandom.Next(minValue, maxValue);

            return scrapValue;
        }

        public static float CalculateApparatusScrapWeight()
        {
            ScrapConfig apparatusConfig = Plugin.Instance.PluginConfig.FetchApparatusConfig();

            if (apparatusConfig.weight.Value < 0)
            {
                // Tell it to use the default.
                return -1;
            }

            return CalculateInternalWeight(apparatusConfig.weight.Value);
        }

        public static ScrapHandedness CalculateApparatusTwoHanded()
        {
            ScrapConfig apparatusConfig = Plugin.Instance.PluginConfig.FetchApparatusConfig();
            return apparatusConfig.twoHanded.Value;
        }

        public static void ApplyApparatusTwoHanded(Item properties)
        {
            switch (CalculateApparatusTwoHanded())
            {
                case ScrapHandedness.OneHanded:
                    DisableItemTwoHanded(properties);
                    break;
                case ScrapHandedness.TwoHanded:
                    EnableItemTwoHanded(properties);
                    break;
                case ScrapHandedness.Default:
                    // Skip.
                    break;
                default:
                    // Skip.
                    break;
            }
        }

        public static ScrapConductivity CalculateApparatusConductive()
        {
            ScrapConfig apparatusConfig = Plugin.Instance.PluginConfig.FetchApparatusConfig();

            return apparatusConfig.conductive.Value;
        }

        public static void ApplyApparatusConductive(Item properties)
        {
            switch (CalculateApparatusConductive())
            {
                case ScrapConductivity.NonConductive:
                    properties.isConductiveMetal = false;
                    break;
                case ScrapConductivity.Conductive:
                    properties.isConductiveMetal = true;
                    break;
                case ScrapConductivity.Default:
                    // Skip.
                    break;
                default:
                    // Skip.
                    break;
            }
        }

        public static bool HasScrapConfig(string item)
        {
            return Plugin.Instance.PluginConfig.HasScrapConfig(item);
        }

        public static int CalculateScrapMinValue(string item)
        {
            ScrapConfig scrapConfig = Plugin.Instance.PluginConfig.FetchScrapConfig(item);

            return (int)Math.Floor(scrapConfig.minValue.Value * BASE_SCRAP_MULTIPLIER);
        }

        public static int CalculateScrapMaxValue(string item)
        {
            ScrapConfig scrapConfig = Plugin.Instance.PluginConfig.FetchScrapConfig(item);

            return (int)Math.Floor(scrapConfig.maxValue.Value * BASE_SCRAP_MULTIPLIER);
        }

        public static float CalculateScrapWeight(string item)
        {
            ScrapConfig scrapConfig = Plugin.Instance.PluginConfig.FetchScrapConfig(item);

            return scrapConfig.weight.Value;
        }

        public static float CalculateScrapInternalWeight(string item)
        {
            return CalculateInternalWeight(CalculateScrapWeight(item));
        }

        public static int CalculateScrapRarity(string item, string scene)
        {
            ScrapConfig scrapConfig = Plugin.Instance.PluginConfig.FetchScrapConfig(item);

            if (!scrapConfig.rarities.ContainsKey(scene))
            {
                return -1;
            }

            var sceneRarity = scrapConfig.rarities[scene];

            if (sceneRarity == null)
            {
                return -1;
            }

            return sceneRarity.Value;
        }

        public static void ApplyItemTwoHanded(string item, Item properties)
        {
            if (!HasScrapConfig(item))
            {
                return;
            }

            var config = Plugin.Instance.PluginConfig.FetchScrapConfig(item);

            switch (config.twoHanded.Value)
            {
                case ScrapHandedness.OneHanded:
                    DisableItemTwoHanded(properties);
                    break;
                case ScrapHandedness.TwoHanded:
                    EnableItemTwoHanded(properties);
                    break;
                case ScrapHandedness.Default:
                    // Skip.
                    break;
                default:
                    // Skip.
                    break;
            }
        }

        public static void ApplyItemConductive(string item, Item properties)
        {
            if (!HasScrapConfig(item))
            {
                return;
            }

            var config = Plugin.Instance.PluginConfig.FetchScrapConfig(item);

            switch (config.conductive.Value)
            {
                case ScrapConductivity.NonConductive:
                    properties.isConductiveMetal = false;
                    break;
                case ScrapConductivity.Conductive:
                    properties.isConductiveMetal = true;
                    break;
                case ScrapConductivity.Default:
                    // Skip.
                    break;
                default:
                    // Skip.
                    break;
            }
        }

        public static void EnableItemTwoHanded(Item properties)
        {
            properties.twoHanded = true;
            properties.twoHandedAnimation = true;
            properties.grabAnim = "HoldLung";
        }

        public static void DisableItemTwoHanded(Item properties)
        {
            properties.twoHanded = false;
            properties.twoHandedAnimation = false;
            properties.grabAnim = "";
        }

        static float CalculateInternalWeight(float weight)
        {
            return (weight / WEIGHT_MULTIPLIER) + 1.0f;
        }
    }
}