using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Events;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal class PanelProcessingService : QuantumServiceBase, IPanelProcessingService
    {

        [Service]
        public IPanelManagerService PanelManager { get; set; }

        [Service]
        public IStaticPanelProcessingService StaticPanelProcessor { get; set; }
        
        [Service]
        public IDynamicPanelProcessingService DynamicPanelProcessor { get; set; }

        [Service]
        public IPanelLayoutManagerService LayoutManager { get; set; }
        
        public PanelProcessingService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        [Handles(typeof(UILoadedEvent))]
        public void OnUILoaded()
        {
            ProcessConfig();

            StaticPanelProcessor.ProcessStaticPanelDefinitions();
            DynamicPanelProcessor.ProcessDynamicPanelDefinitions();
            
            EventAggregator.GetEvent<PanelsLoadedEvent>().Publish(new PanelsLoadedArgs());
        }
        


        #region ConfigProcessing

        private void ProcessConfig()
        {
            var config = PanelManager.DockingConfiguration;
            var configDirectory = config.LayoutSerializationDirectory;
            if(!Directory.Exists(configDirectory))
            {
                throw new DirectoryNotFoundException($"Error : Panel Configuration Directory {configDirectory} not found!");
            }

            var layoutDirectory = Path.Combine(configDirectory, "Layout");
            if(!Directory.Exists(layoutDirectory)) {
                Directory.CreateDirectory(layoutDirectory);
            }

            EventAggregator.Subscribe(config.LayoutSerializationEvent, () => LayoutManager.SaveLayout(layoutDirectory));
            EventAggregator.Subscribe(config.LayoutDeserializationEvent, () =>
            {
                if(File.Exists(Path.Combine(layoutDirectory, "PanelLayout.xml")))
                {
                    LayoutManager.LoadLayout(layoutDirectory);
                }
            });

        }

        #endregion ConfigProcessing

        
    }
}
