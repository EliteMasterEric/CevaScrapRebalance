using System.Collections.Generic;
using BepInEx.Configuration;
using System.Globalization;

namespace CevaScrapRebalance
{
    internal class PluginConfig
    {
        public readonly string[] LEVELS = {
            "Level1Experimentation",
            "Level2Assurance",
            "Level3Vow",
            "Level4March",
            "Level5Rend",
            "Level6Dine",
            "Level7Offense",
            "Level8Titan",
            "Level9Artifice",
            "Level10Adamance",
            "Level11Embrion"
            // "Level12Liquidation"
        };

        public readonly string[] SCRAPS = {
            "Airhorn",
            "Bell",
            "BigBolt",
            "Bottles",
            "Brush",
            "Candy",
            "CashRegister",
            "ChemicalJug",
            "Clock",
            "ClownHorn",
            "Comedy",
            "ControlPad",
            "CookieMoldPan",
            "DustPan",
            "EasterEgg",
            "EggBeater",
            "FancyLamp",
            "Flask",
            "GarbageLid",
            "Gift",
            "GoldBar",
            "GoldenCup",
            "Hairdryer",
            "HomemadeFlashbang",
            "JarOfPickles",
            "LargeAxle",
            "LaserPointer",
            "Magic7Ball",
            "MagnifyingGlass",
            "MetalSheet",
            "Mug",
            "OldPhone",
            "Painting",
            "PerfumeBottle",
            "PillBottle",
            "PlasticCup",
            "PlasticFish",
            "RedSoda",
            "Remote",
            "Ring",
            "RubberDucky",
            "SoccerBall",
            "SteeringWheel",
            "StopSign",
            "TeaKettle",
            "Teeth",
            "ToiletPaper",
            "Toothpaste",
            "ToyCube",
            "ToyRobot",
            "ToyTrain",
            "Tragedy",
            "VTypeEngine",
            "WhoopieCushion",
            "YieldSign",
            "ZedDog",
        };

        ScrapConfig ApparatusScrapConfig;
        Dictionary<string, ScrapConfig> ScrapConfigs;

        // Constructor
        public PluginConfig()
        {
            ScrapConfigs = new Dictionary<string, ScrapConfig>();
        }

        // Bind config values to fields
        public void BindConfig(ConfigFile config)
        {
            // Scrap configs
            ApparatusScrapConfig = BuildScrapConfig(config, "Apparatus", "Apparatus Properties", false);
            
            foreach (var scrap in SCRAPS) {
                BuildAndStoreScrapConfig(config, scrap);
            }

            // Additional properties
            // ShowApparatusValue = config.Bind("Apparatus Properties", "ShowApparatusValue", true, "Whether to force displaying the Apparatus value (rather than ???)");
        }

        void BuildAndStoreScrapConfig(ConfigFile config, string item) {
            ScrapConfigs[item] = BuildScrapConfig(config, item);
        }

        ScrapConfig BuildScrapConfig(ConfigFile config, string item, string category = null, bool useRarities = true) {
            if (category == null) {
                category = item + " Properties";
            }

            return new ScrapConfig(
                config.Bind(category, item + "MinScrapValue", -1, "The minimum scrap value of " + item),
                config.Bind(category, item + "MaxScrapValue", -1, "The minimum scrap value of " + item),
                config.Bind(category, item + "ScrapWeight", -1.0f, "The weight scrap value of " + item),
                useRarities ? BuildScrapConfigRarities(config, item, category) : null,
                config.Bind(category, item + "ScrapTwoHanded", ScrapHandedness.Default, "Whether the item " + item + " is two handed"),
                config.Bind(category, item + "ScrapConductivity", ScrapConductivity.Default, "Whether the item " + item + " is conductive")
            );
        }

        public Dictionary<string, ConfigEntry<int>> BuildScrapConfigRarities(ConfigFile config, string item, string category) {
            var result = new Dictionary<string, ConfigEntry<int>>();

            foreach (var level in LEVELS) {
                result[level] = config.Bind(category, item + "ScrapRarity" + level, -1, "The scrap rarity of " + item + " on " + level + "(-1 to use default)");
            }

            return result;
        }

        public ScrapConfig FetchApparatusConfig() {
            return ApparatusScrapConfig;
        }

        public ScrapConfig FetchScrapConfig(string item) {
            return ScrapConfigs[item];
        }

        public bool HasScrapConfig(string item) {
            return ScrapConfigs.ContainsKey(item);
        }

        public static string SanitizeScrapKey(string key) {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(key.Replace("-", " ").ToLower()).Replace(" ", "");
        }
    }

    readonly record struct ScrapConfig(
        ConfigEntry<int> minValue,
        ConfigEntry<int> maxValue,
        ConfigEntry<float> weight,

        Dictionary<string, ConfigEntry<int>> rarities,

        ConfigEntry<ScrapHandedness> twoHanded,
        ConfigEntry<ScrapConductivity> conductive
    );

    enum ScrapHandedness {
        Default,
        OneHanded,
        TwoHanded
    }

    enum ScrapConductivity {
        Default,
        Conductive,
        NonConductive
    }
}
