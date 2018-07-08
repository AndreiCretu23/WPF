using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quantum.Command
{
    public interface ICommandBase : ICommand
    {
        void RaiseCanExecuteChanged();
        CommandMetadataCollection CommandMetadata { get; }
    }

    public interface IManagedCommand : ICommandBase
    {
        MenuMetadataCollection MainMenuMetadata { get; }
    }

    public interface IMultiManagedCommand
    {
        MultiMenuMetadataCollection MenuMetadata { get; }
    }

    public interface ISubCommand : ICommandBase
    {
        SubMenuMetadataCollection SubCommandMetadata { get; }
    }
}
