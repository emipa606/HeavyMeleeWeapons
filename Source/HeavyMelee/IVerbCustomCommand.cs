using Verse;

namespace HeavyMelee;

public interface IVerbCustomCommand
{
    Command_VerbTarget GetTargetCommand(Command_VerbTarget old);
}