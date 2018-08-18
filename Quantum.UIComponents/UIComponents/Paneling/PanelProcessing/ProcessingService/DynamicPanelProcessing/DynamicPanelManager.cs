using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Services;
using Quantum.Metadata;
using Xceed.Wpf.AvalonDock.Layout;
using Quantum.Utils;

namespace Quantum.UIComponents
{
    internal class DynamicPanelManager : QuantumServiceBase, IDynamicPanelManager
    {
        public IDynamicPanelDefinition Definition { get; private set; }
        
        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }
        
        public DynamicPanelManager(IObjectInitializationService initSvc, IDynamicPanelDefinition definition)
            : base(initSvc)
        {
            Definition = definition;
        }

        public void ProcessDefinition()
        {

        }
        

    }
}
