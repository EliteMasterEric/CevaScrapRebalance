using System;
using System.Collections;
using HarmonyLib;

namespace CevaScrapRebalance.Patch {

	[HarmonyPatch(typeof(LungProp))]
	[HarmonyPatch("DisconnectFromMachinery")]
	class LungPropDisconnectFromMachineryPatch {
        public static void Postfix(LungProp __instance, ref IEnumerator __result) {
            try {
		        Action prefixAction = () => { 
					Console.WriteLine("--> beginning");
				};
		        Action postfixAction = () => { 
					Console.WriteLine("--> end");
					HandleApparatusGrab(__instance);
				};
		        Action<object> preItemAction = (item) => { Console.WriteLine($"--> before {item}"); };
		        Action<object> postItemAction = (item) => { Console.WriteLine($"--> after {item}"); };
		        Func<object, object> itemAction = (item) =>
		        {
		        	var newItem = item + "+";
		        	Console.WriteLine($"--> item {item} => {newItem}");
		        	return newItem;
		        };
		        var myEnumerator = new SimpleEnumerator()
		        {
		        	enumerator = __result,
		        	prefixAction = prefixAction,
		        	postfixAction = postfixAction,
		        	preItemAction = preItemAction,
		        	postItemAction = postItemAction,
		        	itemAction = itemAction
		        };
		        __result = myEnumerator.GetEnumerator();
            }
            catch (Exception e)
            {
                Plugin.Instance.PluginLogger.LogError("Error in MimicDoorAttackPatch.Postfix: " + e);
                Plugin.Instance.PluginLogger.LogError(e.StackTrace);
            }
        }

        private static void HandleApparatusGrab(LungProp apparatus) {
			Plugin.Instance.PluginLogger.LogInfo("Apparatus retrieved, fixing properties...");

			NetworkRPC.BroadcastApparatusValueServerRpc(apparatus.gameObject);
        }
	}

	class SimpleEnumerator : IEnumerable
	{
		public IEnumerator enumerator;
		public Action prefixAction, postfixAction;
		public Action<object> preItemAction, postItemAction;
		public Func<object, object> itemAction;
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public IEnumerator GetEnumerator()
		{
			prefixAction();
			while (enumerator.MoveNext())
			{
				var item = enumerator.Current;
				preItemAction(item);
				yield return itemAction(item);
				postItemAction(item);
			}
			postfixAction();
		}
	}
}