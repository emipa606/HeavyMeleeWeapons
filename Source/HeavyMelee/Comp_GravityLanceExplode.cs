using Verse;
using Verse.Sound;

namespace HeavyMelee;

public class Comp_GravityLanceExplode : ThingComp
{
    public int ticksTillDetonation = -1;

    public CompProperties_GravityLanceExplode Props => (CompProperties_GravityLanceExplode)props;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (!respawningAfterLoad)
        {
            ticksTillDetonation = Props.explosionAfterTicks;
        }
    }

    public override void CompTick()
    {
        base.CompTick();
        ticksTillDetonation -= 1;
        if (ticksTillDetonation == 0)
        {
            GenExplosion.DoExplosion(
                parent.Position,
                parent.Map,
                Props.explosionRadius,
                Props.damageDef,
                parent,
                Props.damageAmount,
                Props.armourPenetration);
            Props.impactSoundDef.PlayOneShot(
                SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map))
            );
            parent.Destroy();
        }
        else
        {
            if (ticksTillDetonation <= 240)
            {
                if (ticksTillDetonation == 240)
                {
                    Props.countDownSoundDef4Sec.PlayOneShot(
                        SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map))
                    );
                }
            }
            else if ((ticksTillDetonation - 240) % 60 == 0)
            {
                Props.countDownSoundDef.PlayOneShot(
                    SoundInfo.InMap(new TargetInfo(parent.Position, parent.Map))
                );
            }
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref ticksTillDetonation, "ticksTillDetonation");
    }
}