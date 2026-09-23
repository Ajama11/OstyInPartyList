using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;

namespace OstyInPartyList.OstyInPartyListCode.OstyLikes;

public class BaseOstyLike // Not abstract so that the default icon value can be used
{
    public virtual string MonsterName => "ReplaceThis";
    public virtual string Icon => ImageHelper.GetImagePath("atlases/power_atlas.sprites/guarded_power.tres");

    public bool MatchName(Creature creature)
    {
        return creature.Monster?.GetType().FullName == MonsterName;
    }

    public bool ShouldCount(Creature creature)
    {
        return MatchName(creature) && ExtraConditionalToCount(creature);
    }

    public virtual bool ExtraConditionalToCount(Creature creature)
    {
        return true;
    }
}