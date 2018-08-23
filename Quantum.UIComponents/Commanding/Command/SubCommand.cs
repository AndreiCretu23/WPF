using Quantum.Metadata;

namespace Quantum.Command
{
    public class SubCommand : StaticMetadataOwnerCommand<SubCommandMetadataCollection, ISubCommandMetadata>, ISubCommand
    {
    }
    
}
