using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using OstyInPartyList.OstyInPartyListCode.Singletons;

namespace OstyInPartyList.OstyInPartyListCode.Patches;

[HarmonyPatch]
public static class PartyListPatches
{
    [HarmonyPatch(
        typeof(NMultiplayerPlayerState),
        nameof(NMultiplayerPlayerState.UpdateHighlightedState))]
    [HarmonyPostfix]
    public static void UpdateHighlightedStatePostfix(NMultiplayerPlayerState __instance)
    {
        if (__instance._isHighlighted)
        {
            PartyListSingleton.OstyHpBar[__instance].FadeInHpLabel(0.1f);
        }
        else
        {
            PartyListSingleton.OstyHpBar[__instance].FadeOutHpLabel(0.5f, 0);
        }
    }
    
    [HarmonyPatch(
        typeof(NMultiplayerPlayerState),
        nameof(NMultiplayerPlayerState.RefreshValues))]
    [HarmonyPostfix]
    public static void RefreshValuesPostfix(NMultiplayerPlayerState __instance)
    {
        PartyListSingleton.UpdateOstyValues(__instance);
    }
    
    [HarmonyPatch(
        typeof(NMultiplayerPlayerState),
        nameof(NMultiplayerPlayerState.BlockChanged))]
    [HarmonyPostfix]
    public static void BlockChangedPostfix(NMultiplayerPlayerState __instance)
    {
        PartyListSingleton.UpdateOstyValues(__instance);
    }
    
    [HarmonyPatch(
        typeof(NMultiplayerPlayerState),
        nameof(NMultiplayerPlayerState.OnCombatEnded))]
    [HarmonyPostfix]
    public static void OnCombatEndedPostfix(NMultiplayerPlayerState __instance)
    {
        PartyListSingleton.OstyHpBar[__instance].Visible = false;
    }
    
    [HarmonyPatch(
        typeof(CreatureCmd),
        nameof(CreatureCmd.LoseMaxHp),
        MethodType.Async)]
    [HarmonyPostfix]
    public static void LoseMaxHpPostfix()
    {
        foreach (var state in NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes)
        {
            PartyListSingleton.UpdateOstyValues(state);
        }
    }
}