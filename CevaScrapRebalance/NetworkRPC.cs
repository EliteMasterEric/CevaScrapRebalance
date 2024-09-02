using StaticNetcodeLib;
using Unity.Netcode;

#nullable enable

namespace CevaScrapRebalance
{
    /**
     * Uses StaticNetcodeLib to perform RPCs
     */
    [StaticNetcode]
    class NetworkRPC
    {
        /**
         * The client has retrieved the Apparatus, and wishes to request a weight and value override from the server.
         * Call this function on any client, and it will be invoked on the server.
         */
        [ServerRpc(RequireOwnership = false)]
        public static void BroadcastApparatusValueServerRpc(NetworkObjectReference gameObjectRef) {
            // Calculate the new properties of the apparatus
            Plugin.Instance.PluginLogger.LogDebug($"Server received Apparatus sync request: ({gameObjectRef})");

			var apparatusValue = ScrapValueCalculator.CalculateApparatusScrapValue();
            float apparatusWeight = ScrapValueCalculator.CalculateApparatusScrapWeight();
            ScrapHandedness twoHanded = ScrapValueCalculator.CalculateApparatusTwoHanded();
            ScrapConductivity conductive = ScrapValueCalculator.CalculateApparatusConductive();

            BroadcastApparatusValueClientRpc(gameObjectRef, apparatusValue, apparatusWeight, twoHanded, conductive);
        }

        /**
         * The server has received a cause of death from a client (possibly itself),
         * and wishes to report it back to all clients.
         * Call this function on the server, and it will be invoked on all clients.
         */
        [ClientRpc]
        public static void BroadcastApparatusValueClientRpc(NetworkObjectReference gameObjectRef, int value, float weight, ScrapHandedness twoHanded, ScrapConductivity conductive) {
            Plugin.Instance.PluginLogger.LogDebug($"Client received Apparatus sync request: ({gameObjectRef}) ({value}, {weight}, {twoHanded}, {conductive})");
            
            if (!gameObjectRef.TryGet(out NetworkObject gameObject)) {
                Plugin.Instance.PluginLogger.LogError($"Could not retrieve Apparatus from NetworkObjectReference: ({gameObjectRef})");
            }

            LungProp apparatus = gameObject.GetComponent<LungProp>();

            if (weight > 0) {
				Plugin.Instance.PluginLogger.LogInfo("Setting Apparatus weight to " + weight);
				apparatus.itemProperties.weight = weight;
			} else {
				Plugin.Instance.PluginLogger.LogInfo("Leaving Apparatus weight as default");
			}

            if (value > 0) {
                Plugin.Instance.PluginLogger.LogInfo("Setting Apparatus scrap value to " + value);
                apparatus.SetScrapValue(value);
            } else {
                Plugin.Instance.PluginLogger.LogInfo("Leaving Apparatus scrap value as default");
            }

            switch (twoHanded) {
                case ScrapHandedness.OneHanded:
                    Plugin.Instance.PluginLogger.LogInfo("Setting Apparatus to two-handed...");
                    ScrapValueCalculator.DisableItemTwoHanded(apparatus.itemProperties);
                    break;
                case ScrapHandedness.TwoHanded:
                    Plugin.Instance.PluginLogger.LogInfo("Setting Apparatus to one-handed...");
                    ScrapValueCalculator.EnableItemTwoHanded(apparatus.itemProperties);
                    break;
                case ScrapHandedness.Default:
                    Plugin.Instance.PluginLogger.LogInfo("Leaving Apparatus handedness as default");
                    break;
                default:
                    Plugin.Instance.PluginLogger.LogInfo("Leaving Apparatus handedness as default");
                    break;
            }

            switch (conductive) {
                case ScrapConductivity.NonConductive:
                    Plugin.Instance.PluginLogger.LogInfo("Enabling Apparatus conductivity...");
                    apparatus.itemProperties.isConductiveMetal = false;
                    break;
                case ScrapConductivity.Conductive:
                    Plugin.Instance.PluginLogger.LogInfo("Disabling Apparatus conductivity...");
                    apparatus.itemProperties.isConductiveMetal = true;
                    break;
                case ScrapConductivity.Default:
                    Plugin.Instance.PluginLogger.LogInfo("Leaving Apparatus conductivity as default");
                    break;
                default:
                    Plugin.Instance.PluginLogger.LogInfo("Leaving Apparatus conductivity as default");
                    break;
            }
        }
    }
}