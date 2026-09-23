using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace OstyInPartyList.OstyInPartyListCode.OstyLikes;

public class TheOsty : BaseOstyLike
{
    public static readonly ModelId GotYourBackPower = new("POWER", "THEHEROEXPANSION-GOT_YOUR_BACK_POWER");
    
    public override string MonsterName =>
        typeof(Osty).FullName!; // "MegaCrit.Sts2.Core.Models.Monsters.Osty"

    public override string Icon =>
        ImageHelper.GetImagePath("atlases/power_atlas.sprites/die_for_you_power.tres");

    public override bool ExtraConditionalToCount(Creature creature)
    {
        return (creature.PetOwner?.Creature.GetPower(GotYourBackPower)?.Amount ?? 0) == 0;
    }
}