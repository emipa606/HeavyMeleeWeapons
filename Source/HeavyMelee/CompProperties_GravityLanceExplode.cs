﻿using Verse;

namespace HeavyMelee;

public class CompProperties_GravityLanceExplode : CompProperties
{
    public readonly int explosionAfterTicks = 0;
    public float armourPenetration;
    public SoundDef countDownSoundDef;
    public SoundDef countDownSoundDef4Sec;
    public int damageAmount;
    public DamageDef damageDef;
    public float explosionRadius;
    public SoundDef impactSoundDef;

    public CompProperties_GravityLanceExplode()
    {
        compClass = typeof(Comp_GravityLanceExplode);
    }
}