using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace AttackSwapper
{
    public class Patches
    {
        private static RuntimeAnimatorController OriginalPlayerController;
        private static RuntimeAnimatorController MyNewController;

        [HarmonyPatch(typeof(Player), nameof(Player.Start))]
        static class Player_Init_Load_Animator
        {
           
            static void Postfix(Player __instance)
            {
                if (!AttackSwapperMod.FirstInit)
                {
                    AttackSwapperMod.FirstInit = true;
                    OriginalPlayerController = Utilities.MakeAoc(__instance.m_animator.runtimeAnimatorController, new Dictionary<string,string>(), new());
                    MyNewController = Utilities.MakeAoc(__instance.m_animator.runtimeAnimatorController, AttackSwapperMod.ReplacementAnimationList, AttackSwapperMod.ExternalAnimations);
                }
            }
        }

        [HarmonyPatch(typeof(VisEquipment), nameof(VisEquipment.SetRightHandEquiped))]
        public static class TestEquipPatch
        {
            public static void Postfix(VisEquipment __instance)
            {
                if (__instance.gameObject.GetComponent<Player>() != null)
                {
                    if (__instance.m_rightItemInstance.GetComponent<ItemDrop>().m_itemData.m_shared.m_itemType ==
                        ItemDrop.ItemData.ItemType.TwoHandedWeapon)
                    {
                        Player.m_localPlayer.m_animator.runtimeAnimatorController = MyNewController;
                    }
                    else
                    {
                        Player.m_localPlayer.m_animator.runtimeAnimatorController = OriginalPlayerController;
                    }
                }
            }
        }
    }
}