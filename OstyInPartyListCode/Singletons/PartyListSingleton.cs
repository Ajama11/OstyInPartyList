using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using OstyInPartyList.OstyInPartyListCode.OstyLikes;

namespace OstyInPartyList.OstyInPartyListCode.Singletons;

public class PartyListSingleton() : CustomSingletonModel(HookType.Combat)
{
    public static List<BaseOstyLike> OstyLikes =
    [
        new TheOsty(),
        new Torchhead(),
        new Mortis(),
        new TheKinFollower(),
    ];

    public static readonly AddedNode<NHealthBar, TextureRect> OstyHpBarIcon = new(bar =>
    {
        TextureRect icon = new TextureRect();

        icon.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
        icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        icon.Size = new Vector2(24, 24);
        icon.Position = new Vector2(-22, -4.5f);
        icon.MouseFilter = Control.MouseFilterEnum.Ignore;
        icon.Visible = false;
        
        return icon;
    });
    
    public static readonly AddedNode<NMultiplayerPlayerState, NHealthBar> OstyHpBar = new(state =>
    {
        var hpBar = (NHealthBar) state._healthBar.Duplicate();
        
        hpBar.Position = new Vector2(63, 58);
        hpBar._blockTrackingCreature = state.Player.Creature;
        hpBar.Visible = false;

        return hpBar;
    });

    public override Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.PetOwner == null) return Task.CompletedTask;
        if (!IsOstyLike(creature)) return Task.CompletedTask;

        PossiblyChangeWhoIsTanking(creature);
        
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!IsOstyLike(creature)) return Task.CompletedTask;
        
        PossiblyChangeWhoIsTanking(creature);
        
        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == creature.PetOwner);
        if (state == null) return Task.CompletedTask;
        
        UpdateOstyValues(state);
        
        return Task.CompletedTask;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, CardModel? source, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!IsOstyLike(creature)) return Task.CompletedTask;

        PossiblyChangeWhoIsTanking(creature);
        
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        // MainFile.Logger.Warn($"{power.Owner.Monster?.GetType().FullName} has changed {power.Id}");
        if (power.Id != Mortis.TankingPower &&
            power.Id != TheOsty.GotYourBackPower &&
            power.Id != TheKinFollower.TankingPower)
        {
            return Task.CompletedTask;
        }
        
        if (power.Id == TheOsty.GotYourBackPower && power.Owner.Player != null)
        {
            PossiblyChangeWhoIsTanking(power.Owner, power.Owner.Player);
        }
        else
        {
            PossiblyChangeWhoIsTanking(power.Owner);
        }
        
        return Task.CompletedTask;
    }
    

    public static void CreateOstyBar(NMultiplayerPlayerState state, Creature creature)
    {
        OstyHpBar[state]._creature = null!;
        OstyHpBar[state].SetCreature(creature);
        
        OstyHpBar[state].FadeOutHpLabel(0, 0);
        OstyHpBar[state]._hpLabel.ZIndex = 1;

        state.MoveChild(OstyHpBar[state], 0);
        
        string iconPath = OstyLikes
            .FirstOrDefault(o => o.MatchName(creature))
            ?.Icon ?? new BaseOstyLike().Icon;

        OstyHpBarIcon[OstyHpBar[state]].Texture = PreloadManager.Cache.GetTexture2D(iconPath);
        OstyHpBarIcon[OstyHpBar[state]].Visible = true;
        
        UpdateOstyValues(state);
        OstyHpBar[state].Visible = true;
    }

    public static void UpdateOstyValues(NMultiplayerPlayerState state)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (OstyHpBar[state]._creature == null) return;
        
        OstyHpBar[state].SetHpBarContainerSizeWithOffsetsImmediately(OstyHpBar[state].HpBarContainer.Size with
        {
            X = Math.Min
            (
                Math.Min
                (
                    Math.Max
                    (
                        (OstyHpBar[state]._creature.MaxHp + 20) / 120f * 175f, // 0 to 40 HP
                        OstyHpBar[state]._creature.MaxHp / 80f * 175f // 40 to 100 HP
                    ),
                    (OstyHpBar[state]._creature.MaxHp + 400) / 400f * 175f // 100 to 500 HP
                ),
                (500 + 400) / 400f * 175f // 500+ HP, capped
            )
        });
        
        OstyHpBar[state].RefreshValues();
    }
    
    /// <param name="somePet">The pet in question. If overriding "player", this is ignored, and you can just pass in the player's Creature.</param>
    /// <param name="player">The player who owns the pet. If null/not overridden, will be somePet.PetOwner</param>
    public static void PossiblyChangeWhoIsTanking(Creature somePet, Player? player = null)
    {
        player ??= somePet.PetOwner;
        
        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == player);
        if (state == null) return;

        Creature? ostyLike = GetWhichOstyLikeWillTank(player);
        
        if (ostyLike != null && OstyHpBar[state]._creature != ostyLike)
        {
            CreateOstyBar(state, ostyLike);
        }

        if (ostyLike == null)
        {
            OstyHpBar[state].Visible = false;
            OstyHpBar[state]._creature = null!;
        }
    }

    public static bool IsOstyLike(Creature creature)
    {
        return OstyLikes.Any(o => o.ShouldCount(creature));
    }

    public static Creature? GetWhichOstyLikeWillTank(Player? player)
    {
        return player?.Creature.Pets.FirstOrDefault(c => IsOstyLike(c) && c.IsAlive);
    }
}