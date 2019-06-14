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
        IGlobalCommand OpenDialog { get; }
        IGlobalCommand RemoveLastPanel { get; }

        IGlobalCommand CommandChangeNShortcut { get; }
        IGlobalCommand CommandChangeBShortcut { get; }
        IGlobalCommand CommandClearShortcut { get; }

        IGlobalCommand PanelChangeAShortcut { get; }
        IGlobalCommand PanelChangeQShortcut { get; }
        IGlobalCommand PanelClearShortcut { get; }


        IGlobalCommand Qwerty { get; }
        IGlobalCommand Yolo1Command { get; }
        IGlobalCommand Yolo2Command { get; }

        IGlobalCommand Change1 { get; }
        IGlobalCommand Change2 { get; }
        IGlobalCommand Change3 { get; }
        IGlobalCommand Change4 { get; }
        IGlobalCommand Change5 { get; }
        IGlobalCommand Change6 { get; }
        IGlobalCommand Change7 { get; }
        IGlobalCommand Change8 { get; }
        IGlobalCommand Change9 { get; }
        IGlobalCommand Change10 { get; }

        IGlobalCommand CheckCommand { get; }
        IMultiGlobalCommand RecentCommands { get; }
        IMultiGlobalCommand SubRecentCommands { get; }

        IGlobalCommand HiddenCommand { get; }
        
    }
}
