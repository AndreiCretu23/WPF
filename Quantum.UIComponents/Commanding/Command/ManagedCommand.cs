using Quantum.Metadata;

namespace Quantum.Command
{
    public class ManagedCommand : StaticMetadataOwnerCommand<CommandMetadataCollection, ICommandMetadata>, IManagedCommand
    {
    }
}
