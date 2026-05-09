using GameNetcodeStuff;
using JetpackPocket.Patches;

namespace JetpackPocket
{
    internal static class JetpackHelper
    {
        public struct JetpackState
        {
            public bool HasJetpack;
            public int HasTwoHandedItem;
        }

        private static readonly System.Collections.Generic.Dictionary<ulong, JetpackState> PlayerDictionary = new System.Collections.Generic.Dictionary<ulong, JetpackState>();

        public static bool HasJetpack(PlayerControllerB player)
        {
            if (player == null) return false;
            return PlayerDictionary.TryGetValue(player.playerClientId, out JetpackState val) && val.HasJetpack;
        }

        public static bool HasTwoHandedAndJetpack(PlayerControllerB player)
        {
            return PlayerDictionary.TryGetValue(player.playerClientId, out JetpackState val) && val.HasJetpack && val.HasTwoHandedItem > 0;
        }

        public static bool HasTwoHanded(PlayerControllerB player)
        {
            return PlayerDictionary.TryGetValue(player.playerClientId, out JetpackState val) && val.HasTwoHandedItem > 0;
        }

        private static void SaveState(ulong player, JetpackState value)
        {
            PlayerDictionary[player] = value;
        }

        public static void Rescan(PlayerControllerB player)
        {
            if (player == null) return;
            JetpackState newState = new JetpackState();
            foreach (var slot in player.ItemSlots)
            {
                if (IsJetpack(slot))
                {
                    newState.HasJetpack = true;
                }

                if (IsTwoHanded(slot))
                {
                    newState.HasTwoHandedItem++;
                }
            }

            //utility slot check
            if (IsJetpack(player.ItemOnlySlot))
            {
                newState.HasJetpack = true;
            }

            SaveState(player.playerClientId, newState);
        }

        private static bool IsJetpack(GrabbableObject item)
        {
            if (item == null) return false;
            return item.GetType().Name == "JetpackItem";
        }

        private static bool IsTwoHanded(GrabbableObject item)
        {
            if (item == null) return false;
            return item.itemProperties.twoHanded;
        }

        public static void UpdateHUD(PlayerControllerB __instance)
        {
            if (__instance != null && __instance.IsOwner)
            {
                bool twoHanded = JetpackHelper.HasTwoHanded(__instance);
                bool hasJetpack = JetpackHelper.HasJetpack(__instance);
                HUDManager.Instance.holdingTwoHandedItem.enabled = (twoHanded && !hasJetpack) || (twoHanded && hasJetpack && !ConfigSync.SyncedCarryMultipleTwoHanded);
            }
        }

        public static void UpdateHUD()
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            if (player != null)
            {
                UpdateHUD(player);
            }
        }
    }
}
