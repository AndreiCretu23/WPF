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
                
                var anchorables = DockingView.DockingManager.Layout.Descendents().OfType<LayoutAnchorable>();
                EventAggregator.GetEvent<LayoutLoadedEvent>().Publish(new LayoutLoadedArgs(anchorables));

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
