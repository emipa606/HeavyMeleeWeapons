using RimWorld;
using Verse;

namespace HeavyMelee;

public class Verb_SelfDamagingMelee : Verb_MeleeAttackDamage
{
    protected override bool TryCastShot()
    {
        if (!base.TryCastShot())
        {
            return false;
        }

        EquipmentSource.HitPoints -= EquipmentSource.def.GetModExtension<SelfDamageModExtension>()
            .selfDamageAmountPerAttack;
        if (EquipmentSource.HitPoints > 0)
        {
            return true;
        }

        var bb = ThingMaker.MakeThing(GravityLanceDefOf.PlantedGravityLance);
        GenSpawn.Spawn(bb, CasterPawn.Position, CasterPawn.Map);
        EquipmentSource.Destroy();

        return true;
    }
}