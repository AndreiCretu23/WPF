using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Services;
using Quantum.Metadata;
using System.Collections.ObjectModel;

namespace Quantum.UIComponents
{
    internal class DynamicPanelProcessingService : QuantumServiceBase, IDynamicPanelProcessingService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }
        
        private IObjectInitializationService InitializationService { get; set; }

        public DynamicPanelProcessingService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            InitializationService = initSvc;
        }
        
        public void ProcessDynamicPanelDefinitions()
        {
            var definitions = PanelManager.DynamicPanelDefinitions;
        }
    }
}
