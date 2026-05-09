using HarmonyLib;
using Unity.Netcode;

namespace JetpackPocket.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal static class GameNetworkManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("StartGame")]
        private static void OnStartGame()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsHost) return;

            JetpackPocketPatchBase.logSource.LogInfo("Host started game - sending config to clients.");
            ConfigSync.SendToClients(JetpackPocketPatchBase.instance.JetpackPocketConfigEntry.Value);
        }

        [HarmonyPostfix]
        [HarmonyPatch("StartDisconnect")]
        private static void OnStartDisconnect()
        {
            ConfigSync.SyncedCarryMultipleTwoHanded = JetpackPocketPatchBase.instance.JetpackPocketConfigEntry.Value;
            JetpackPocketPatchBase.logSource.LogInfo("Disconnected — reset synced config to local value.");
        }
    }
}
