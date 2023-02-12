using System.Linq;
using MVCF.Utilities;

namespace HeavyMelee;

public class Verb_UnpoweredMelee : Verbs_ReloadableMelee
{
    public override bool Available()
    {
        var reloadable = EquipmentSource.AllReloadComps().First();
        return reloadable is not { ShotsRemaining: < SHOT_CONSUMPTION } && base.Available();
    }
}