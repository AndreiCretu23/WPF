using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Runtime.InteropServices;

namespace WPF.Panels
{
    [Guid("3AF60523-CED6-4E8B-918B-207647C018FD")]
    public class TargetPanelViewModel : ViewModelBase, ITargetPanelViewModel
    {
        public TargetPanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
    }
}
