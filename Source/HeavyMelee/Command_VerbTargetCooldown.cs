using UnityEngine;
using Verse;

namespace HeavyMelee;

[StaticConstructorOnStartup]
public class Command_VerbTargetCooldown : Command_VerbTarget
{
    private static readonly Texture2D CooldownTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));

    public Command_VerbTargetCooldown(Command_VerbTarget old)
    {
        defaultLabel = old.defaultLabel;
        defaultDesc = old.defaultDesc;
        icon = old.icon;
        iconAngle = old.iconAngle;
        iconOffset = old.iconOffset;
        tutorTag = old.tutorTag;
        verb = old.verb;
    }

    private IVerbCooldown Cooldown => verb as IVerbCooldown;

    protected override GizmoResult GizmoOnGUIInt(Rect butRect, GizmoRenderParms parms)
    {
        disabled = Cooldown.CooldownPercentLeft != 0f;
        disabledReason = disabled ? (string)"HeavyMelee.OnCooldown".Translate() : null;

        var result = base.GizmoOnGUIInt(butRect, parms);

        if (disabled)
        {
            GUI.DrawTexture(butRect.RightPartPixels(butRect.width * Cooldown.CooldownPercentLeft), CooldownTex);
        }

        return result;
    }
}