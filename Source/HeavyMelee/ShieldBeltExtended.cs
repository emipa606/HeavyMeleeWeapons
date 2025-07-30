using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using VEF.Apparels;
using Verse;
using Verse.Sound;

namespace HeavyMelee;

[StaticConstructorOnStartup]
public class ShieldBeltExtended : Apparel_Shield
{
    private const float MinDrawSize = 1.2f;

    private const float MaxDrawSize = 1.55f;

    private const float MaxDamagedJitterDist = 0.05f;

    private const int JitterDurationTicks = 8;

    private static readonly Material BubbleMat =
        MaterialPool.MatFrom("Other/ShieldBubble", ShaderDatabase.Transparent);

    private readonly float ApparelScorePerEnergyMax = 0.25f;

    private readonly float EnergyLossPerDamage = 0.033f;

    private readonly float EnergyOnReset = 0.2f;

    private readonly int KeepDisplayingTicks = 1000;

    private readonly int StartingTicksToReset = 3200;

    private float energy;

    private Vector3 impactAngleVect;

    private int lastAbsorbDamageTick = -9999;

    private int lastKeepDisplayTick = -9999;

    private int ticksToReset = -1;

    public float EnergyMax => this.GetStatValue(StatDefOf.EnergyShieldEnergyMax) * 3.2f;

    private float EnergyGainPerTick => this.GetStatValue(StatDefOf.EnergyShieldRechargeRate) / 60f;

    public float Energy => energy;

    private bool compAllowsActive => this.TryGetComp<Comp_ExtendedShield>().shieldActive;

    public ShieldState ShieldState => ticksToReset > 0 ? ShieldState.Resetting : ShieldState.Active;

    private bool ShouldDisplay
    {
        get
        {
            var wearer = Wearer;
            return wearer.Spawned && !wearer.Dead && !wearer.Downed && (wearer.InAggroMentalState ||
                                                                        wearer.Drafted ||
                                                                        wearer.Faction
                                                                            .HostileTo(Faction.OfPlayer) &&
                                                                        !wearer.IsPrisoner ||
                                                                        Find.TickManager.TicksGame <
                                                                        lastKeepDisplayTick + KeepDisplayingTicks);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref energy, "energy");
        Scribe_Values.Look(ref ticksToReset, "ticksToReset", -1);
        Scribe_Values.Look(ref lastKeepDisplayTick, "lastKeepDisplayTick");
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        foreach (var gizmo in base.GetWornGizmos())
        {
            yield return gizmo;
        }

        if (Find.Selector.SingleSelectedThing == Wearer)
        {
            yield return new Gizmo_EnergyShieldExtendedStatus
            {
                shield = this
            };
        }
    }

    public override float GetSpecialApparelScoreOffset()
    {
        return EnergyMax * ApparelScorePerEnergyMax;
    }

    protected override void Tick()
    {
        base.Tick();
        if (Wearer == null)
        {
            energy = 0f;
            return;
        }

        if (ShieldState == ShieldState.Resetting)
        {
            ticksToReset--;
            if (ticksToReset <= 0)
            {
                Reset();
            }
        }
        else if (ShieldState == ShieldState.Active)
        {
            energy += EnergyGainPerTick;
            if (energy > EnergyMax)
            {
                energy = EnergyMax;
            }
        }
    }

    public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
    {
        if (ShieldState != ShieldState.Active || !compAllowsActive)
        {
            return false;
        }

        if (dinfo.Def == DamageDefOf.EMP)
        {
            energy = 0f;
            Break();
            return false;
        }

        if (!dinfo.Def.isRanged && !dinfo.Def.isExplosive)
        {
            return false;
        }

        energy -= dinfo.Amount * EnergyLossPerDamage;
        if (energy < 0f)
        {
            Break();
        }
        else
        {
            AbsorbedDamage(dinfo);
        }

        return true;
    }

    public void KeepDisplaying()
    {
        lastKeepDisplayTick = Find.TickManager.TicksGame;
    }

    private void AbsorbedDamage(DamageInfo dinfo)
    {
        SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
        impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
        var loc = Wearer.TrueCenter() + (impactAngleVect.RotatedBy(180f) * 0.5f);
        var num = Mathf.Min(10f, 2f + (dinfo.Amount / 10f));
        FleckMaker.Static(loc, Wearer.Map, FleckDefOf.ExplosionFlash, num);
        var num2 = (int)num;
        for (var i = 0; i < num2; i++)
        {
            FleckMaker.ThrowDustPuff(loc, Wearer.Map, Rand.Range(0.8f, 1.2f));
        }

        lastAbsorbDamageTick = Find.TickManager.TicksGame;
        KeepDisplaying();
    }

    private void Break()
    {
        SoundDef.Named("EnergyShield_Broken").PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
        FleckMaker.Static(Wearer.TrueCenter(), Wearer.Map, FleckDefOf.ExplosionFlash, 12f);
        for (var i = 0; i < 6; i++)
        {
            FleckMaker.ThrowDustPuff(
                Wearer.TrueCenter() + (Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) *
                                       Rand.Range(0.3f, 0.6f)), Wearer.Map, Rand.Range(0.8f, 1.2f));
        }

        energy = 0f;
        ticksToReset = StartingTicksToReset;
    }

    private void Reset()
    {
        if (Wearer.Spawned)
        {
            SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
            FleckMaker.ThrowLightningGlow(Wearer.TrueCenter(), Wearer.Map, 3f);
        }

        ticksToReset = -1;
        energy = EnergyOnReset;
    }

    public override void DrawWornExtras()
    {
        base.DrawWornExtras();
        if (ShieldState != ShieldState.Active || !compAllowsActive || !ShouldDisplay)
        {
            return;
        }

        var num = Mathf.Lerp(MinDrawSize, MaxDrawSize, energy);
        var vector = Wearer.Drawer.DrawPos;
        vector.y = AltitudeLayer.MoteOverhead.AltitudeFor();
        var num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
        if (num2 < JitterDurationTicks)
        {
            var num3 = (JitterDurationTicks - num2) / 8f * MaxDamagedJitterDist;
            vector += impactAngleVect * num3;
            num -= num3;
        }

        num *= 2.0f;
        var angle = (float)Rand.Range(0, 360);
        var s = new Vector3(num, 1f, num);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
    }
}