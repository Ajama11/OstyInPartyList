using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace OstyInPartyList.OstyInPartyListCode.OstyLikes;

public class TheKinFollower : BaseOstyLike
{
    public static readonly ModelId TankingPower = new ("POWER", "THEKINSTS2-KIN_GUARD_POWER");
    
    public override string MonsterName =>
        "TheKinStS2.Code.Monsters.KinPackFollower";
    
    public override string Icon =>
        "res://OstyInPartyList/images/ui/kin_follower_icon.png"; // The icon from the actual mod ends up tiny

    public static bool HasTankingPower(Creature creature)
    {
        return (creature.GetPower(TankingPower)?.Amount ?? 0) > 0;
    }

    public override bool ExtraConditionalToCount(Creature creature)
    {
        return !creature.PetOwner!.IsOstyAlive && HasTankingPower(creature);
    }
}