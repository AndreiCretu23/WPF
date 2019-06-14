using Quantum.Command;
using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Runtime.InteropServices;
using WPF.Commands;

namespace WPF.Panels
{
    [Guid("577953C7-1DAC-4564-9E49-9790113C42B2")]
    public class ActivePanelViewModel : ViewModelBase, IActivePanelViewModel
    {
        [Selection]
        public SelectedNumber SelectedNumber { get; set; }

        [InvalidateOn(typeof(SelectedNumber))]
        public string Description => $"Selected Numer is : {SelectedNumber.Value.ToString()}";
        

        public ActivePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {

        }


        [Command(typeof(ICommonCommands))]
        public IGlobalCommand Change2 { get; set; }

        [Command(typeof(ICommonCommands))]
        public IGlobalCommand Change3 { get; set; }
    }
}
