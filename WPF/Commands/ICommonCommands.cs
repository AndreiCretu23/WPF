using Quantum.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Commands
{
    public interface ICommonCommands : ICommandContainer
    {
        IManagedCommand Qwerty { get; }
        IManagedCommand Yolo1Command { get; }
        IManagedCommand Yolo2Command { get; }

        IManagedCommand Change1 { get; }
        IManagedCommand Change2 { get; }
        IManagedCommand Change3 { get; }
        IManagedCommand Change4 { get; }
        IManagedCommand Change5 { get; }
        IManagedCommand Change6 { get; }
        IManagedCommand Change7 { get; }
        IManagedCommand Change8 { get; }
        IManagedCommand Change9 { get; }
        IManagedCommand Change10 { get; }

        IManagedCommand CheckCommand { get; }
        IMultiManagedCommand RecentCommands { get; }

        IManagedCommand HiddenCommand { get; }
        
    }
}
