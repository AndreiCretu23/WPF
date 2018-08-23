using Quantum.Metadata;

namespace Quantum.Command
{
    public class MultiManagedCommand : MultiStaticMetadataOwnerCommand<ISubCommand, MultiCommandMetadataCollection, IMultiCommandMetadata>, IMultiManagedCommand
    {
    }
}
