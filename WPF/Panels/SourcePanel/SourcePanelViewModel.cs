using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Runtime.InteropServices;

namespace WPF.Panels
{
    [Guid("00D8BA2E-ED54-45A6-8098-0FC992C35627")]
    public class SourcePanelViewModel : ViewModelBase, ISourcePanelViewModel
    {
        public SourcePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
    }
}
