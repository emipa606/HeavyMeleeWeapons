using Verse;

namespace HeavyMelee;

public class CompProperties_ExtendedShield : CompProperties
{
    public readonly string shieldIcon = "";
    public GraphicData shieldActiveGraphic;
    public string shieldToggleDesc;

    public string shieldToggleLabel;

    public CompProperties_ExtendedShield()
    {
        compClass = typeof(Comp_ExtendedShield);
    }
}