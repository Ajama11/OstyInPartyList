using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace OstyInPartyList.OstyInPartyListCode.OstyLikes;

public class Mortis : BaseOstyLike
{
    public static readonly ModelId TankingPower = new ("POWER", "AVEMUJICA-DO_NOT_FEAR_DEATH_POWER");
    
    public override string MonsterName =>
        "AveMujica.AveMujicaCode.Cards.Dolls.MortisDoll";
    
    public override string Icon =>
        "res://AveMujica/images/powers/do_not_fear_death_power.png";

    public override bool ExtraConditionalToCount(Creature creature)
    {
        return (creature.GetPower(TankingPower)?.Amount ?? 0) > 0;
    }
}