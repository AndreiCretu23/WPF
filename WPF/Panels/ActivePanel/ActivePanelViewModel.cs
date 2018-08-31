using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Runtime.InteropServices;

namespace WPF.Panels
{
    [Guid("577953C7-1DAC-4564-9E49-9790113C42B2")]
    public class ActivePanelViewModel : ViewModelBase, IActivePanelViewModel
    {

        public ActivePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {

        }

    }
}
