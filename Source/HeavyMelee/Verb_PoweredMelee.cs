using System.Linq;
using MVCF.Utilities;

namespace HeavyMelee;

public class Verb_PoweredMelee : Verbs_ReloadableMelee
{
    protected override bool TryCastShot()
    {
        var reloadable = EquipmentSource.AllReloadComps().First();

        if (reloadable == null)
        {
            return false;
        }

        if (!base.TryCastShot())
        {
            return false;
        }

        reloadable.ShotsRemaining -= SHOT_CONSUMPTION;
        return true;
    }

    public override bool Available()
    {
        var reloadable = EquipmentSource.AllReloadComps().First();

        return reloadable is { ShotsRemaining: >= SHOT_CONSUMPTION } && base.Available();
    }
}