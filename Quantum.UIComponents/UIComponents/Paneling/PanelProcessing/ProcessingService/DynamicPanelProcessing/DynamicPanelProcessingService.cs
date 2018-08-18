using Quantum.Services;
using System.Collections.ObjectModel;

namespace Quantum.UIComponents
{
    internal class DynamicPanelProcessingService : QuantumServiceBase, IDynamicPanelProcessingService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }
        
        private IObjectInitializationService InitializationService { get; set; }

        private Collection<IDynamicPanelManager> dynamicPanelManagers = new Collection<IDynamicPanelManager>();
        
        public DynamicPanelProcessingService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            InitializationService = initSvc;
        }
        
        public void ProcessDynamicPanelDefinitions()
        {
            var definitions = PanelManager.DynamicPanelDefinitions;

            foreach (var def in definitions)
            {
                dynamicPanelManagers.Add(new DynamicPanelManager(InitializationService, def));
            }

            foreach(var manager in dynamicPanelManagers)
            {
                manager.ProcessDefinition();
            }
        }
    }
}
