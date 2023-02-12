using System.Linq;
using RimWorld;
using Verse;

namespace HeavyMelee;

public class DamageWorker_PsychicSmash : DamageWorker_Blunt
{
    public override DamageResult Apply(DamageInfo dinfo, Thing thing)
    {
        var result = base.Apply(dinfo, thing);
        if (thing is Pawn pawn && dinfo is { Weapon: { }, Instigator: Pawn pawn2 } && !pawn.Dead &&
            !pawn.Downed)
        {
            var weapon = pawn2.equipment.AllEquipmentListForReading.Concat(pawn2.apparel.WornApparel)
                .FirstOrDefault(t => t.def == dinfo.Weapon);
            var comp = weapon.TryGetComp<CompPsychicShock>();
            if (comp is { ShockOnNext: true })
            {
                var hediff = HediffMaker.MakeHediff(HediffDefOf.PsychicShock, pawn);
                pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource)
                    .TryRandomElement(out var part);
                pawn.health.AddHediff(hediff, part);
                comp.ShockOnNext = false;
            }
        }

        return result;
    }
}