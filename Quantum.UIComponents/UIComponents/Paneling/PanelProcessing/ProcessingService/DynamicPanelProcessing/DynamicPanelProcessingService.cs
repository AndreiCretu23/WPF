using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class DynamicPanelProcessingService : ServiceBase, IDynamicPanelProcessingService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }
        
        private Collection<IDynamicPanelManager> DynamicPanelManagers = new Collection<IDynamicPanelManager>();
        
        public DynamicPanelProcessingService(IObjectInitializationService initSvc)
            : base(initSvc)
        {            
        }
        
        public void ProcessDynamicPanelDefinitions()
        {
            var definitions = PanelManager.DynamicPanelDefinitions;

            foreach (var def in definitions)
            {
                DynamicPanelManagers.Add(new DynamicPanelManager(InitializationService, def));
            }

            foreach(var manager in DynamicPanelManagers)
            {
                manager.ProcessDefinition();
            }
        }

        [Handles(typeof(BringDynamicPanelIntoViewRequest))]
        public void OnBringIntoView(BringDynamicPanelIntoViewArgs args)
        {
            args.ViewModel.AssertNotNull(nameof(args.ViewModel));

            IDynamicPanelManager manager = null;
            try
            {
                manager = DynamicPanelManagers.Single(mgr => mgr.Definition.ViewModel == args.PanelViewModel ||
                                                             mgr.Definition.IViewModel == args.PanelViewModel);
            }
            catch(InvalidOperationException)
            {
                throw new Exception($"Error bringing an instance of {args.ViewModel} into view. " +
                                    $"No DynamicPanelDefinition that has that associated ViewModel type has been registered");
            }

            manager.BringPanelIntoView(args.ViewModel);
        }
    }
}
