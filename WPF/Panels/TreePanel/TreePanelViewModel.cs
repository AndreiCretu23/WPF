using Quantum.Services;
using Quantum.UIComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Panels
{
    [Guid("50924313-BDE0-41D6-A0D5-C8E86CA1EF00")]
    public class TreePanelViewModel : ViewModelBase, ITreePanelViewModel
    {
        public TreePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
    }
}
