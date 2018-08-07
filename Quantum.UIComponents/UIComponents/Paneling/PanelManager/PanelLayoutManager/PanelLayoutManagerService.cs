using Quantum.Services;
using Quantum.Utils;
using System.IO;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Quantum.UIComponents
{
    internal class PanelLayoutManagerService : QuantumServiceBase, IPanelLayoutManagerService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }

        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }
        
        public PanelLayoutManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public bool LoadLayout(string layoutFileName)
        {
            if (DockingView == null) return false;

            var dockingManager = DockingView.DockingManager;

            dockingManager.UpdateLayout();
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
            if (File.Exists(layoutFileName))
            {
                using (var stream = new FileStream(layoutFileName, FileMode.Open, FileAccess.Read))
                {
                    layoutSerializer.Deserialize(stream);
                }

                return true;
            }
            else
            {
                throw new FileNotFoundException($"Error : The specified Layout File does not exist.");
            }
        }
        

        public bool SaveLayout(string layoutFileName)
        {
            if (DockingView == null) return false;

            var dockingManager = DockingView.DockingManager;
            dockingManager.UpdateLayout();

            var serializer = new XmlLayoutSerializer(dockingManager);
            IOUtils.DeleteIfExists(layoutFileName);

            using (var stream = new FileStream(layoutFileName, FileMode.Create))
            {
                serializer.Serialize(stream);
            }

            return true;
        }
    }
}
