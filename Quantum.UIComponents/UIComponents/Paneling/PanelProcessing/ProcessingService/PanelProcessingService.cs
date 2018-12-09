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
    internal class PanelProcessingService : ServiceBase, IPanelProcessingService
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
            
            if(config.SerializesLayout) 
            {
                EventAggregator.Subscribe(config.LayoutSerializationEvent, () =>
                {
                    var directory = config.LayoutSerializationDirectory;
                    if(!Directory.Exists(directory)) {
                        throw new DirectoryNotFoundException($"Error : Cannot serialize the panels' layout : The serialization directory defined in the config : " +
                                                             $"{directory ?? String.Empty} does not exist.");

                    }
                    var fileName = config.LayoutSerializationFileName;
                    if(fileName.IsNullOrEmpty()) {
                        throw new Exception("$Error : Cannot serialize the panels' layout : The filename provided by the docking configuration is not valid (It's either null or empty).");
                    }

                    var layoutFile = Path.ChangeExtension(Path.Combine(directory, fileName), ".xml");
                    LayoutManager.SaveLayout(layoutFile);
                });

                EventAggregator.Subscribe(config.LayoutDeserializationEvent, () =>
                {
                    var directory = config.LayoutSerializationDirectory;
                    var fileName = config.LayoutSerializationFileName;
                    if(directory.IsNullOrEmpty() || fileName.IsNullOrEmpty()) {
                        throw new Exception($"Error : Cannot attempt to deserialize the panels' layout : the provided serialization directory or fileName (in the docking configuration " +
                                            $"located in the defined in the panel manager) is either null or empty. These values need to be valid. There might not be a " +
                                            $"saved layout file (The application is loaded for the first time for example), but even then, these values need to be valid " +
                                            $"in order for the framework to at least search for them.");
                    }

                    var layoutFile = Path.ChangeExtension(Path.Combine(directory, fileName), ".xml");
                    if(File.Exists(layoutFile)) {
                        LayoutManager.LoadLayout(layoutFile);
                    }
                });
            }
        }

        #endregion ConfigProcessing

        
    }
}
