using Quantum.Services;
using Quantum.Utils;
using System.IO;
using System.Linq;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Quantum.UIComponents
{
    internal class PanelLayoutManagerService : QuantumServiceBase, IPanelLayoutManagerService
    {
        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }
        
        public PanelLayoutManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public bool LoadLayout(string directory)
        {
            if (DockingView == null) return false;

            var layoutFileName = Path.Combine(directory, "PanelLayout.xml");
            
            var dockingManager = DockingView.DockingManager;

            dockingManager.UpdateLayout();
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
            if (File.Exists(layoutFileName))
            {
                using (var stream = new FileStream(layoutFileName, FileMode.Open, FileAccess.Read))
                {
                    layoutSerializer.Deserialize(stream);
                }
                
                // On occasion, avalon serializes some closed panels (DynamicPanels), leading to the mess-up of the associated dynamicPanelCollection layout restoration.
                // To avoid this, we simply re-close them.
                var anchorables = DockingView.DockingManager.Layout.Descendents().OfType<LayoutAnchorable>().ToList();
                foreach(var anch in anchorables)
                {
                    if (anch.Content == null)
                    {
                        anch.Close();
                    }
                }

                EventAggregator.GetEvent<LayoutLoadedEvent>().Publish(new LayoutLoadedArgs(DockingView.DockingManager.Layout.Descendents().OfType<LayoutAnchorable>()));

                return true;
            }
            else
            {
                throw new FileNotFoundException($"Error : The specified Layout File does not exist.");
            }
        }


        public bool SaveLayout(string directory)
        {
            if (DockingView == null) return false;
            
            var dockingManager = DockingView.DockingManager;
            dockingManager.UpdateLayout();
            
            var layoutFileName = Path.Combine(directory, "PanelLayout.xml");
            IOUtils.DeleteIfExists(layoutFileName);

            var serializer = new XmlLayoutSerializer(dockingManager);
            using (var stream = new FileStream(layoutFileName, FileMode.Create))
            {
                serializer.Serialize(stream);
            }
            return true;
        }
    }
}
