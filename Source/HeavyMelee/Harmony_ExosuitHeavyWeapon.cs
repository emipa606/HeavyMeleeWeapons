using System.Collections.Generic;
using HarmonyLib;
using HeavyWeapons;
using RimWorld;
using UnityEngine;
using Verse;
using VFECore;
using Patch_FloatMenuMakerMap = HeavyWeapons.Patch_FloatMenuMakerMap;

namespace HeavyMelee;

[StaticConstructorOnStartup]
public static class Harmony_ExosuitHeavyWeapon
{
    public static readonly HashSet<string> SA_HeavyWeaponHediffString = [];
    public static readonly HashSet<HediffDef> SA_HeavyWeaponableHediffDefs = [];
    public static readonly HashSet<string> SA_HeavyWeaponThingString = [];
    public static readonly HashSet<ThingDef> SA_HeavyWeaponThingDefs = [];

    public static readonly HashSet<HeavyWeapon>
        SA_HeavyWeaponExtentionInstances =
        [
        ]; //each of the heavy weapon has a unique instance of this class, so this can be used to keep track of which weapon

    public static readonly List<Thing> SA_PawnsCheck = [];
    public static readonly List<Apparel> SA_ConcurrencyErrorSafetyNet = [];

    static Harmony_ExosuitHeavyWeapon()
    {
        SA_HeavyWeaponHediffString.Add("ExoskeletonSuit");
        SA_HeavyWeaponHediffString.Add("Trunken_hediff_ExoskeletonArmor");
        SA_HeavyWeaponHediffString.Add("FSFImplantTorsoWorker");
        SA_HeavyWeaponHediffString.Add("FSFImplantTorsoSpeed");
        SA_HeavyWeaponHediffString.Add("FSFImplantTorsoPsychic");
        SA_HeavyWeaponHediffString.Add("TP_Combat_Exosuit");
        SA_HeavyWeaponHediffString.Add("TP_Combat_Exosuit_MK_A");
        SA_HeavyWeaponHediffString.Add("TP_Combat_Exosuit_MK_B");
        SA_HeavyWeaponHediffString.Add("TP_Combat_Exosuit_MK_C");
        SA_HeavyWeaponHediffString.Add("TP_Combat_Exosuit_MK_D");

        //SA_HeavyWeaponHediffString.AddRange(ExosuitPatchLoaderDefOf.exosuitPatchLoader.CountAsExosuitHediff);
        foreach (var str in SA_HeavyWeaponHediffString)
        {
            var hdd = DefDatabase<HediffDef>.GetNamed(str, false);
            SA_HeavyWeaponableHediffDefs.Add(hdd);
        }

        //SA_HeavyWeaponHediffString.AddRange(ExosuitPatchLoaderDefOf.exosuitPatchLoader.CountAsHeavyWeapon);
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_HeavyMonoSword");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_FlameLance");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_PsychicWarhammer");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_ZeusSword");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_RocketHammer");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_SagittariusMight");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_PersonaHeavyMonoSword");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_PersonaFlameLance");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_PersonaPsychicWarhammer");
        SA_HeavyWeaponThingString.Add("HMW_MeleeWeapon_PersonaZeusSword");
        //SA_HeavyWeaponThingString.Add("HMW_GuardianShield");

        foreach (var str in SA_HeavyWeaponThingString)
        {
            var hdd = DefDatabase<ThingDef>.GetNamed(str, false);
            if (hdd == null) //Log.Warning("Could not find the def of " + str);
            {
                continue;
            }

            SA_HeavyWeaponThingDefs.Add(hdd);
            var HW = hdd.GetModExtension<HeavyWeapon>();
            if (HW == null)
            {
                //Log.Warning(str + " has no HeavyWeapon extention !");
            }
            else
            {
                SA_HeavyWeaponExtentionInstances.Add(HW);
            }
        }

        var harmony = new Harmony("ExoHW.ExosuitHeavyWeapon");
        //harmony.PatchAll();

        harmony.Patch(
            AccessTools.Method(typeof(Patch_FloatMenuMakerMap.AddHumanlikeOrders_Fix), "CanEquip"),
            null,
            new HarmonyMethod(typeof(Harmony_ExosuitHeavyWeapon), nameof(CanEquipPostFix)));

        harmony.Patch(
            AccessTools.Method(typeof(Pawn_EquipmentTracker), "GetGizmos"),
            null,
            new HarmonyMethod(typeof(Harmony_ExosuitHeavyWeapon), nameof(GetExtraEquipmentGizmosPassThrough)));

        harmony.Patch(
            AccessTools.Method(typeof(Thing), "TakeDamage"),
            new HarmonyMethod(typeof(Harmony_ExosuitHeavyWeapon), nameof(TakeDamageExtendedShield)));

        harmony.Patch(
            AccessTools.Method(typeof(Apparel_Shield), "DrawWornExtras"),
            null,
            new HarmonyMethod(typeof(Harmony_ExosuitHeavyWeapon), nameof(DrawWornExtraPost))
        );

        //Harmony_YayoCombat_OversizedWeaponTranspiler.tryPatch_YC_OWT(harmony);
    }

    public static void DrawWornExtraPost(Apparel_Shield __instance)
    {
        __instance.TryGetComp<Comp_ExtendedShield>()?.PostDraw();
    }

    public static IEnumerable<Gizmo> GetExtraEquipmentGizmosPassThrough(IEnumerable<Gizmo> values,
        Pawn_EquipmentTracker __instance)
    {
        foreach (var giz in values)
        {
            yield return giz;
        }

        if (!PawnAttackGizmoUtility.CanShowEquipmentGizmos() || !__instance.pawn.IsColonistPlayerControlled)
        {
            yield break;
        }

        var list = __instance.AllEquipmentListForReading;
        foreach (var eq in list)
        {
            var smp = eq.def.GetModExtension<SagittariusMightPlantModExtention>();
            if (smp == null)
            {
                continue;
            }

            var eq1 = eq;
            yield return new Command_Action
            {
                defaultLabel = smp.label,
                defaultDesc = smp.description,
                icon = ContentFinder<Texture2D>.Get(smp.texPath),
                action = delegate
                {
                    var bb = ThingMaker.MakeThing(GravityLanceDefOf.PlantedGravityLance);
                    GenSpawn.Spawn(bb, __instance.pawn.Position, __instance.pawn.Map);
                    eq1.Destroy();
                }
            };
        }
    }

    public static void CanEquipPostFix(Pawn pawn, HeavyWeapon options, ref bool __result)
    {
        if (__result || !SA_HeavyWeaponExtentionInstances.Contains(options))
        {
            return;
        }

        if (pawn is not { health: not null })
        {
            return;
        }

        foreach (var hed in pawn.health.hediffSet.hediffs)
        {
            if (!SA_HeavyWeaponableHediffDefs.Contains(hed.def))
            {
                continue;
            }

            __result = true;
            return;
        }
    }

    public static bool canBeShielded(Pawn p)
    {
        return p.Faction != null;
    }

    public static IEnumerable<Apparel> DefenderPawnShields(Pawn basePawn, DamageInfo di)
    {
        var map = basePawn.Map;
        if (map == null)
        {
            yield break;
        }

        var dirIntForm = (int)(di.Angle * 16 / 360.0f) % 16;
        Vector3 checkVector;
        switch (dirIntForm)
        {
            case 15: //east
            case 0:
            {
                checkVector = new Vector3(0, 0, -1);
                break;
            }
            case 1:
            case 2:
            {
                checkVector = new Vector3(-1, 0, -1);
                break;
            }
            case 3: //north
            case 4:
            {
                checkVector = new Vector3(-1, 0, 0);
                break;
            }
            case 5:
            case 6:
            {
                checkVector = new Vector3(-1, 0, 1);
                break;
            }
            case 7: //west
            case 8:
            {
                checkVector = new Vector3(0, 0, 1);
                break;
            }
            case 9:
            case 10:
            {
                checkVector = new Vector3(1, 0, 1);
                break;
            }
            case 11: //south
            case 12:
            {
                checkVector = new Vector3(1, 0, 0);
                break;
            }
            case 13:
            case 14:
            {
                checkVector = new Vector3(1, 0, -1);
                break;
            }
            default:
            {
                Log.Warning("Shield Calculation : Angle out of range");
                checkVector = default;
                break;
            }
        }

        //Log.Warning("Angle Calc " + checkVector2 + " / " + di.Angle);
        SA_PawnsCheck.Clear();
        SA_ConcurrencyErrorSafetyNet.Clear();
        var baseFaction = basePawn.Faction;
        _ = checkVector.ToIntVec3() + basePawn.Position;
        //SA_PawnsCheck.AddRange(map.thingGrid.ThingsAt(iv3));
        for (var i = 0; i < 9; i++)
        {
            SA_PawnsCheck.AddRange(
                map.thingGrid.ThingsAt(basePawn.Position + new IntVec3((i / 3) - 1, 0, (i % 3) - 1)));
        }

        //SA_PawnsCheck.AddRange(map.thingGrid.ThingsAt(basePawn.Position));
        foreach (var thing in SA_PawnsCheck)
        {
            if (thing is not Pawn { apparel: not null } pawn || pawn.Faction != baseFaction || pawn.Downed)
            {
                continue;
            }

            foreach (var app in pawn.apparel.WornApparel)
            {
                var compC = app.TryGetComp<Comp_ExtendedShield>();
                if (compC is { shieldActive: true } && app is ShieldBeltExtended
                    {
                        ShieldState: ShieldState.Active
                    })
                {
                    SA_ConcurrencyErrorSafetyNet.Add(app);
                }
            }
        }

        foreach (var app in SA_ConcurrencyErrorSafetyNet)
        {
            yield return app;
        }
    }

    public static bool TakeDamageExtendedShield(Thing __instance, DamageInfo dinfo,
        ref DamageWorker.DamageResult __result)
    {
        if (__instance is not Pawn p)
        {
            return true;
        }

        var amountNow = dinfo.Amount;
        foreach (var apparel in DefenderPawnShields(p, dinfo)
                ) //int damage = Convert.ToInt32(Mathf.Min(amountNow, damageMe.HitPoints));
            //damageMe.HitPoints -= damage;
            //DamageInfo ddClone = new DamageInfo(dinfo);
            //ddClone.SetAmount(damage);
            //damageMe.TakeDamage(ddClone);
            //amountNow -= damage;
        {
            var damageMe = (ShieldBeltExtended)apparel;
            if (!damageMe.CheckPreAbsorbDamage(dinfo))
            {
                continue;
            }

            amountNow = 0;
            break;
        }

        //dinfo.SetAmount(amountNow);
        if (amountNow != 0)
        {
            return true;
        }

        __result = new DamageWorker.DamageResult();
        return false;
    }
}