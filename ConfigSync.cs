using LethalNetworkAPI;

namespace JetpackPocket
{
    internal static class ConfigSync
    {
        private const string MessageId = ModInfo.modGUID + ".CarryMultipleTwoHanded";

        internal static bool SyncedCarryMultipleTwoHanded = false;
        private static LNetworkMessage<bool> networkedMessage;

        public static void SetNetworkMessage()
        {
            SyncedCarryMultipleTwoHanded = JetpackPocketPatchBase.instance.JetpackPocketConfigEntry.Value;
            networkedMessage = LNetworkMessage<bool>.Connect(MessageId, onClientReceived: OnReceivedFromHost);
        }
        public static void SendToClients(bool value)
        {
            SyncedCarryMultipleTwoHanded = value;
            networkedMessage.SendClients(value);
            JetpackPocketPatchBase.logSource.LogInfo($"Sent CarryMultipleTwoHanded = {value}, to all clients.");
        }

        private static void OnReceivedFromHost(bool value)
        {
            SyncedCarryMultipleTwoHanded = value;
            JetpackPocketPatchBase.logSource.LogInfo($"Received CarryMultipleTwoHanded = {value}, from host.");
            JetpackHelper.UpdateHUD();
        }
    }
}
