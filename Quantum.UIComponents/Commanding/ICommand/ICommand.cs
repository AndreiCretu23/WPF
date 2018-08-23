using Quantum.Metadata;

namespace Quantum.Command
{
    public interface IDelegateCommand : IStaticCommand
    {
    }

    public interface IManagedCommand : IStaticMetadataOwnerCommand<CommandMetadataCollection, ICommandMetadata>
    {
    }

    public interface IMultiManagedCommand : IMultiStaticMetadataOwnerCommand<ISubCommand, MultiCommandMetadataCollection, IMultiCommandMetadata>
    {
    }

    public interface ISubCommand : IStaticMetadataOwnerCommand<SubCommandMetadataCollection, ISubCommandMetadata>
    {
    }
}
