using BepInEx.Configuration;

namespace GarageDoorFix
{
    public class PluginConfig
    {
        ConfigEntry<int> GarageDoorChance;
        ConfigEntry<bool> DoorDropOnlyOnce;

        // Constructor
        public PluginConfig()
        {
        }

        // Bind config values to fields
        public void BindConfig(ConfigFile _config)
        {
            GarageDoorChance = _config.Bind("General", "GarageDoorChance", 3, "The chance for the garage door to drop, as a percentage.");
            DoorDropOnlyOnce = _config.Bind("General", "DoorDropOnlyOnce", true, "If true, the garage door will only drop once per round.");
        }

        public int GetGarageDoorChance()
        {
            return GarageDoorChance.Value;
        }

        public bool ShouldDoorDropOnlyOnce()
        {
            return DoorDropOnlyOnce.Value;
        }
    }
}
