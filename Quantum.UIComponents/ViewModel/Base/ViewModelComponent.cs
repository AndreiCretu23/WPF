using Quantum.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Provides the base class for any ViewModel that has access to the container, services and events, 
    /// and also connects the view model to the command component of the framework, extracting the metadata and 
    /// providing DataContext support for ContextMenu and Shortcuts.
    /// </summary>
    public abstract class ViewModelComponent : ViewModelBase
    {
        public ViewModelComponent(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
    }
}
