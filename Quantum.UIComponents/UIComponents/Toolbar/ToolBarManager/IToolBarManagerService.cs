using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    public interface IToolBarManagerService
    {
        void RegisterToolBarDefinition<ITView, TView, ITViewModel, TViewModel>(ToolBarDefinition<ITView, TView, ITViewModel, TViewModel> toolbarDefinition)
            where TView : UserControl, ITView, new()
            where ITView : class
            where TViewModel : class, ITViewModel
            where ITViewModel : class;

        void RegisterToolBarDefinition(IToolBarDefinition toolBarDefinition);

        IEnumerable<IToolBarDefinition> GetToolBarDefinitions();
    }
}
