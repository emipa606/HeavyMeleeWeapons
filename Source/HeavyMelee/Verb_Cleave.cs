using RimWorld;
using Verse;

namespace HeavyMelee;

public class Verb_Cleave : Verb, IVerbTick, IVerbCustomCommand, IVerbCooldown
{
    public const float COOLDOWN = 12f;
    private int cooldownTicksLeft;
    public float CooldownPercentLeft => cooldownTicksLeft / (float)COOLDOWN.SecondsToTicks();

    public Command_VerbTarget GetTargetCommand(Command_VerbTarget old)
    {
        return new Command_VerbTargetCooldown(old);
    }

    public void Tick()
    {
        if (cooldownTicksLeft > 0)
        {
            cooldownTicksLeft--;
        }
    }

    protected override bool TryCastShot()
    {
        if (cooldownTicksLeft > 0)
        {
            return false;
        }

        foreach (var thing in GenRadial.RadialDistinctThingsAround(Caster.Position, Caster.Map, verbProps.range,
                     false))
        {
            ApplyDamage(thing);
        }

        cooldownTicksLeft = COOLDOWN.SecondsToTicks();
        return true;
    }

    public override bool Available()
    {
        return cooldownTicksLeft <= 0 && base.Available();
    }

    private void ApplyDamage(Thing target)
    {
        var damageInfo = new DamageInfo(verbProps.meleeDamageDef, verbProps.meleeDamageBaseAmount,
            verbProps.meleeArmorPenetrationBase, -1f, caster, null,
            EquipmentSource != null ? EquipmentSource.def : CasterPawn.def);
        damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
        if (HediffCompSource != null)
        {
            damageInfo.SetWeaponHediff(HediffCompSource.Def);
        }

        damageInfo.SetAngle((target.Position - CasterPawn.Position).ToVector3());
        var log = new BattleLogEntry_MeleeCombat(RulePackDef.Named("Maneuver_Slash_MeleeHit"), false, CasterPawn,
            target, ImplementOwnerType, ReportLabel, EquipmentSource?.def, HediffCompSource?.Def,
            LogEntryDefOf.MeleeAttack);
        target.TakeDamage(damageInfo).AssociateWithLog(log);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref cooldownTicksLeft, "cooldownTicksLeft_Cleave");
    }
}