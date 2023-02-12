using Verse;

namespace HeavyMelee;

public class CompProperties_PsychicShock : CompProperties
{
    public float PsyfocusCost;

    public CompProperties_PsychicShock()
    {
        compClass = typeof(CompPsychicShock);
    }
}