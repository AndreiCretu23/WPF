using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    internal class ToolBarManagerService : QuantumServiceBase, IToolBarManagerService
    {
        public ToolBarManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public void RegisterToolBarDefinition<ITView, TView, ITViewModel, TViewModel>(ToolBarDefinition<ITView, TView, ITViewModel, TViewModel> toolbarDefinition)
            where TView : UserControl, ITView
            where TViewModel : ITViewModel
        {
        }
    }
}
