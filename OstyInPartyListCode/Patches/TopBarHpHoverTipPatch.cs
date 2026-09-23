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
        
        #region KinFollower
        int kinFollowerCurrent = 0;
        int kinFollowerMax = 0;
        bool kinFollowerPresent = false;
        bool kinFollowerTanking = false;
        
        var kinFollower = __instance._player.Creature.Pets.ToList()
            .Find(TheKinFollower.HasTankingPower);
        if (kinFollower != null)
        {
            kinFollowerCurrent = kinFollower.CurrentHp;
            kinFollowerMax = kinFollower.MaxHp;
            kinFollowerPresent = true;
            
            if (__instance._player.Creature.Pets.Count > 1 &&
                PartyListSingleton.GetWhichOstyLikeWillTank(__instance._player) == kinFollower)
            {
                kinFollowerTanking = true;
            }
        }
        
        description.Add("KinFollowerCurrent", kinFollowerCurrent);
        description.Add("KinFollowerMax", kinFollowerMax);
        description.Add("KinFollowerPresent", kinFollowerPresent);
        description.Add("KinFollowerTanking", kinFollowerTanking);
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
        List<bool> layers = CreateNewLineOrdering(
            ostyPresent,
            torchheadPresent,
            kinFollowerPresent,
            dolorisPresent,
            mortisPresent,
            timorisPresent,
            amorisPresent
        );
        
        bool ostyNewLine =        layers[5] && ostyPresent;
        bool torchheadNewLine =   layers[4] && torchheadPresent;
        bool kinFollowerNewLine = layers[3] && kinFollowerPresent;
        bool dolorisNewLine =     layers[2] && dolorisPresent;
        bool mortisNewLine =      layers[1] && mortisPresent;
        bool timorisNewLine =     layers[0] && timorisPresent;
        
        description.Add("OstyNewLine", ostyNewLine);
        description.Add("TorchheadNewLine", torchheadNewLine);
        description.Add("KinFollowerNewLine", kinFollowerNewLine);
        description.Add("DolorisNewLine", dolorisNewLine);
        description.Add("MortisNewLine", mortisNewLine);
        description.Add("TimorisNewLine", timorisNewLine);
        #endregion
        
        var tips = NHoverTipSet.CreateAndShow(__instance, new HoverTip(title, description));
        tips?.SetGlobalPosition(__instance.GlobalPosition + new Vector2(0, __instance.Size.Y + 20));

        return false;
    }

    public static List<bool> CreateNewLineOrdering(params bool[] args)
    {
        List<bool> reverseArgs = (((IEnumerable<bool>) args).Reverse()).ToList();

        List<bool> list = [reverseArgs[0]];

        for (int i = 1; i < reverseArgs.Count; i++)
        {
            list.Add(reverseArgs[i] || list[i - 1]);
        }

        return list;
    }
}