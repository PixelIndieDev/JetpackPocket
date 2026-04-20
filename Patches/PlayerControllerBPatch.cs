using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace JetpackPocket.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        // item switch states
        [HarmonyPatch("GrabObjectClientRpc")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void GrabObjectClientRpcPatch(PlayerControllerB __instance)
        {
            JetpackHelper.Rescan(__instance);
            JetpackHelper.UpdateHUD(__instance);
        }

        [HarmonyPatch("ThrowObjectClientRpc")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void ThrowObjectClientRpcPatch(PlayerControllerB __instance)
        {
            JetpackHelper.Rescan(__instance);
            JetpackHelper.UpdateHUD(__instance);
        }

        [HarmonyPatch("PlaceObjectClientRpc")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void PlaceObjectClientRpcPatch(PlayerControllerB __instance)
        {
            JetpackHelper.Rescan(__instance);
            JetpackHelper.UpdateHUD(__instance);
        }

        [HarmonyPatch("DropAllHeldItems")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void DropAllHeldItemsPatch(PlayerControllerB __instance)
        {
            JetpackHelper.Rescan(__instance);
            JetpackHelper.UpdateHUD(__instance);
        }

        [HarmonyPatch("DespawnHeldObjectClientRpc")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void DespawnHeldObjectClientRpcPatch(PlayerControllerB __instance)
        {
            JetpackHelper.Rescan(__instance);
            JetpackHelper.UpdateHUD(__instance);
        }

        // item scrolling/switching
        [HarmonyPatch("ScrollMouse_performed")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void ScrollPatch(PlayerControllerB __instance)
        {
            if (!__instance.IsOwner || !__instance.isPlayerControlled) return;

            if (__instance.inTerminalMenu) return;

            if (__instance.twoHanded && JetpackHelper.HasJetpack(__instance))
            {
                __instance.twoHanded = false;
            }

            JetpackHelper.UpdateHUD(__instance);
        }

        [HarmonyPatch("UseUtilitySlot_performed")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void tabPatch(PlayerControllerB __instance)
        {
            if (!__instance.IsOwner || !__instance.isPlayerControlled) return;

            if (__instance.inTerminalMenu) return;

            if (__instance.twoHanded && JetpackHelper.HasJetpack(__instance))
            {
                __instance.twoHanded = false;
            }

            JetpackHelper.UpdateHUD(__instance);
        }

        // block pickup second heavy/2-handed item
        [HarmonyPatch("BeginGrabObject")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static bool BeginGrabObjectPatch(PlayerControllerB __instance)
        {
            if (JetpackPocketPatchBase.instance.JetpackPocketConfigEntry.Value) return true;

            if (__instance.twoHanded) return true;

            if (!__instance.IsOwner || !__instance.isPlayerControlled) return true;

            Ray interactRay = new Ray(__instance.gameplayCamera.transform.position, __instance.gameplayCamera.transform.forward);
            const int objectMask = 1073742656;
            if (!Physics.Raycast(interactRay, out RaycastHit hit, __instance.grabDistance, objectMask) || hit.collider.gameObject.layer == 8 || hit.collider.tag != "PhysicsProp" || __instance.sinkingValue > 0.73f || Physics.Linecast(__instance.gameplayCamera.transform.position, hit.collider.transform.position + __instance.transform.up * 0.16f, 1073741824, QueryTriggerInteraction.Ignore))
            {
                return true;
            }
            
            // we hit an item
            if (JetpackHelper.HasTwoHanded(__instance)) {
                //check pickup for two handed
                GrabbableObject target = hit.collider.transform.gameObject.GetComponent<GrabbableObject>();

                if (target != null && target.itemProperties.twoHanded)
                {
                    JetpackHelper.UpdateHUD(__instance);
                    return false;
                }   
            }
            return true;
        }

        [HarmonyPatch("SwitchToItemSlot")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        static void SwitchToItem(PlayerControllerB __instance)
        {
            JetpackHelper.Rescan(__instance);
            JetpackHelper.UpdateHUD(__instance);
        }
    }
}
