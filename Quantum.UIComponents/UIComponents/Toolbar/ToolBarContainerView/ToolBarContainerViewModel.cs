using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Metadata;
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
    internal class ToolBarContainerViewModel : ViewModelBase, IToolBarContainerViewModel
    {
        private IObjectInitializationService InitializationService { get; set; }

        [Service]
        public IToolBarManagerService ToolBarManager { get; set; }
        
        public ToolBarContainerViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            InitializationService = initSvc;
        }
        
        // Content
        public IEnumerable<ToolBarViewModel> Children { get { return CreateToolBarContent(); } }
        
        private IEnumerable<ToolBarViewModel> CreateToolBarContent()
        {
            var toolBarDefinitions = ToolBarManager.GetToolBarDefinitions();
            foreach(var definition in toolBarDefinitions)
            {
                var contentView = (UserControl)Activator.CreateInstance(definition.View);
                var contentViewModel = Container.Resolve(definition.IViewModel);
                contentView.DataContext = contentViewModel;

                var toolBarViewModel = new ToolBarViewModel(InitializationService)
                {
                    Band = definition.Band, 
                    BandIndex = definition.BandIndex, 
                    VisibilityDelegate = definition.Visibility, 
                    Content = contentView
                };
                
                foreach(var metadata in definition.ToolBarMetadata)
                {
                    metadata.IfIs((AutoInvalidateOnEvent e) => EventAggregator.Subscribe(e.EventType, () => toolBarViewModel.RaiseVisibilityChanged(), ThreadOption.UIThread, true));
                    metadata.IfIs((AutoInvalidateOnSelection s) => EventAggregator.Subscribe(s.SelectionType, () => toolBarViewModel.RaiseVisibilityChanged(), ThreadOption.UIThread, true));
                }

                yield return toolBarViewModel;
            }
        }
    }
}
