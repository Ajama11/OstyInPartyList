using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.sts2.Core.Nodes.TopBar;
using OstyInPartyList.OstyInPartyListCode.OstyLikes;
using OstyInPartyList.OstyInPartyListCode.Singletons;

namespace OstyInPartyList.OstyInPartyListCode.Patches;

[HarmonyPatch(typeof(NTopBarHp))]
public static class TopBarHpHoverTipPatch
{
    // HORRENDOUS CODE BELOW
    // NO GODS TRAVERSE THIS LAND
    // FOR THEY WOULD HAVE STOPPED ME
    
    public const string Doloris = "AveMujica.AveMujicaCode.Cards.Dolls.DolorisDoll";
    public const string Timoris = "AveMujica.AveMujicaCode.Cards.Dolls.TimorisDoll";
    public const string Amoris = "AveMujica.AveMujicaCode.Cards.Dolls.AmorisDoll";
    
    [HarmonyPatch("OnFocus")]
    [HarmonyPrefix]
    public static bool OnFocusPrefix(NTopBarHp __instance)
    {
        if (__instance._player == null) return true;
        if (__instance._player.Creature.Pets.Count == 0) return true;
        
        const string str = "OSTYINPARTYLIST-PET_HP";

        LocString title = HoverTipFactory.L10NStatic(str + ".title");
        LocString description = HoverTipFactory.L10NStatic(str + ".description");

        #region Osty
        int ostyCurrent = 0;
        int ostyMax = 0;
        bool ostyPresent = false;
        bool ostyTanking = false;
        
        if (__instance._player.Osty != null)
        {
            var osty = __instance._player.Osty!;
            ostyCurrent = osty.CurrentHp;
            ostyMax = osty.MaxHp;
            ostyPresent = true;

            if (__instance._player.Creature.Pets.Count > 1 &&
                PartyListSingleton.GetWhichOstyLikeWillTank(__instance._player) == osty)
            {
                ostyTanking = true;
            }
        }
        
        description.Add("OstyCurrent", ostyCurrent);
        description.Add("OstyMax", ostyMax);
        description.Add("OstyPresent", ostyPresent);
        description.Add("OstyTanking", ostyTanking);
        #endregion
        
        #region Torchhead
        int torchheadCurrent = 0;
        bool torchheadPresent = false;
        bool torchheadTanking = false;
        
        var torchhead = __instance._player.Creature.Pets.ToList()
            .Find(c => new Torchhead().MatchName(c));
        if (torchhead != null)
        {
            torchheadCurrent = torchhead.CurrentHp;
            torchheadPresent = true;
            
            if (__instance._player.Creature.Pets.Count > 1 &&
                PartyListSingleton.GetWhichOstyLikeWillTank(__instance._player) == torchhead)
            {
                torchheadTanking = true;
            }
        }
        
        description.Add("TorchheadCurrent", torchheadCurrent);
        description.Add("TorchheadPresent", torchheadPresent);
        description.Add("TorchheadTanking", torchheadTanking);
        #endregion
        
        #region Ave Mujica
        int dolorisCurrent = 0;
        bool dolorisPresent = false;
        
        var doloris = __instance._player.Creature.Pets.ToList()
            .Find(c => c.Monster?.GetType().FullName == Doloris);
        if (doloris != null)
        {
            dolorisCurrent = doloris.CurrentHp;
            dolorisPresent = true;
        }
        
        description.Add("DolorisCurrent", dolorisCurrent);
        description.Add("DolorisPresent", dolorisPresent);
        
        
        int mortisCurrent = 0;
        bool mortisPresent = false;
        bool mortisTanking = false;
        
        var mortis = __instance._player.Creature.Pets.ToList()
            .Find(c => new Mortis().MatchName(c));
        if (mortis != null)
        {
            mortisCurrent = mortis.CurrentHp;
            mortisPresent = true;
            
            if (__instance._player.Creature.Pets.Count > 1 &&
                PartyListSingleton.GetWhichOstyLikeWillTank(__instance._player) == mortis)
            {
                mortisTanking = true;
            }
        }
        
        description.Add("MortisCurrent", mortisCurrent);
        description.Add("MortisPresent", mortisPresent);
        description.Add("MortisTanking", mortisTanking);
        
        
        int timorisCurrent = 0;
        bool timorisPresent = false;
        
        var timoris = __instance._player.Creature.Pets.ToList()
            .Find(c => c.Monster?.GetType().FullName == Timoris);
        if (timoris != null)
        {
            timorisCurrent = timoris.CurrentHp;
            timorisPresent = true;
        }
        
        description.Add("TimorisCurrent", timorisCurrent);
        description.Add("TimorisPresent", timorisPresent);
        
        
        int amorisCurrent = 0;
        bool amorisPresent = false;
        
        var amoris = __instance._player.Creature.Pets.ToList()
            .Find(c => c.Monster?.GetType().FullName == Amoris);
        if (amoris != null)
        {
            amorisCurrent = amoris.CurrentHp;
            amorisPresent = true;
        }
        
        description.Add("AmorisCurrent", amorisCurrent);
        description.Add("AmorisPresent", amorisPresent);
        #endregion

        #region New Line Fuckery
        bool ostyNewLine = ostyPresent && (torchheadPresent || dolorisPresent || mortisPresent || timorisPresent || amorisPresent);
        bool torchheadNewLine = torchheadPresent && (dolorisPresent || mortisPresent || timorisPresent || amorisPresent);
        bool dolorisNewLine = dolorisPresent && (mortisPresent || timorisPresent || amorisPresent);
        bool mortisNewLine = mortisPresent && (timorisPresent || amorisPresent);
        bool timorisNewLine = timorisPresent && amorisPresent;
        
        description.Add("OstyNewLine", ostyNewLine);
        description.Add("TorchheadNewLine", torchheadNewLine);
        description.Add("DolorisNewLine", dolorisNewLine);
        description.Add("MortisNewLine", mortisNewLine);
        description.Add("TimorisNewLine", timorisNewLine);
        #endregion
        
        var tips = NHoverTipSet.CreateAndShow(__instance, new HoverTip(title, description));
        tips?.SetGlobalPosition(__instance.GlobalPosition + new Vector2(0, __instance.Size.Y + 20));

        return false;
    }
}