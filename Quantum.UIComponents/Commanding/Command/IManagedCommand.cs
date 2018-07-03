using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quantum.Command
{
    public interface IManagedCommand : ICommand
    {
        void RaiseCanExecuteChanged();
        CommandMetadataCollection CommandMetadata { get; }
    }

    public interface IMainMenuCommand : IManagedCommand
    {
        MainMenuMetadataCollection MainMenuMetadata { get; }
    }
}
