using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Events;
using Quantum.Services;
using Quantum.Utils;

namespace Quantum.UIComponents
{
    internal class PanelProcessingService : QuantumServiceBase, IPanelProcessingService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }
        
        [Service]
        public IPanelLayoutManagerService LayoutManager { get; set; }
        
        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }
        
        public PanelProcessingService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        private bool IsUILoaded = false;

        [Handles(typeof(UILoadedEvent))]
        public void OnUILoaded()
        {
            IsUILoaded = true;
            
        }

    }
}
