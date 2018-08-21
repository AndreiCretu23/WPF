using Quantum.Metadata;
using System.Windows.Input;

namespace Quantum.Command
{
    public interface ICommandBase : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public interface IManagedCommand : ICommandBase
    {
        CommandMetadataCollection Metadata { get; }
    }

    public interface IMultiManagedCommand
    {
        MultiMenuMetadataCollection MenuMetadata { get; }
    }

    public interface ISubCommand : ICommandBase
    {
        CommandMetadataCollection Metadata { get; }
        SubMenuMetadataCollection SubCommandMetadata { get; }
    }
}
