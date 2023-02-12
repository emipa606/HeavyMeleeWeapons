using Verse;

namespace HeavyMelee;

public class CompPsychicShock : ThingComp
{
    public bool ShockOnNext;
    public CompProperties_PsychicShock Props => props as CompProperties_PsychicShock;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref ShockOnNext, "ShockOnNext");
    }
}