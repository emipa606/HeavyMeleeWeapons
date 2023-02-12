using Verse;

namespace HeavyMelee;

public class CompProperties_ExtendedShield : CompProperties
{
    public GraphicData shieldActiveGraphic;
    public string shieldIcon = "";
    public string shieldToggleDesc;

    public string shieldToggleLabel;

    public CompProperties_ExtendedShield()
    {
        compClass = typeof(Comp_ExtendedShield);
    }
}