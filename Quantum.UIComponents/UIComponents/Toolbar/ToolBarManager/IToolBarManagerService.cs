using System.Collections.Generic;
using System.Windows.Controls;

namespace Quantum.UIComponents
{
    public interface IToolBarManagerService
    {
        /// <summary>
        /// Registers the given definition in the ToolBarManager. The framework will process this definition and create the ToolBarView accordingly.
        /// </summary>
        /// <typeparam name="ITView"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="ITViewModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="toolbarDefinition"></param>
        void RegisterToolBarDefinition<ITView, TView, ITViewModel, TViewModel>(ToolBarDefinition<ITView, TView, ITViewModel, TViewModel> toolbarDefinition)
            where TView : UserControl, ITView, new()
            where ITView : class
            where TViewModel : class, ITViewModel
            where ITViewModel : class;

        /// <summary>
        /// Registers the given definition in the ToolBarManager. The framework will process this definition and create the ToolBarView accordingly.
        /// <paramref name="toolBarDefinition">This Parameter must be of the generic type ToolBarDefinition, because the framework requires that because of the type constraints</paramref>
        /// </summary>
        void RegisterToolBarDefinition(IToolBarDefinition toolBarDefinition);
        
        /// <summary>
        /// Returns a collection containing all registered ToolBarDefinitions. Changing any values returned by this method is not safe.
        /// This method is used internally to communicate between the actual ToolBarView and the registered ToolBarDefinition collections.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IToolBarDefinition> GetToolBarDefinitions();
    }
}
