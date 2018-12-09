using Quantum.Services;
using Quantum.Utils;
using System.IO;
using System.Linq;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Quantum.UIComponents
{
    internal class PanelLayoutManagerService : ServiceBase, IPanelLayoutManagerService
    {
        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }
        
        public PanelLayoutManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public void LoadLayout(string layoutFileName)
        {            
            var dockingManager = DockingView.DockingManager;

            dockingManager.UpdateLayout();
            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(dockingManager);
            
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
        }


        public void SaveLayout(string layoutFileName)
        {
            var dockingManager = DockingView.DockingManager;
            dockingManager.UpdateLayout();
            
            IOUtils.DeleteIfExists(layoutFileName);

            var serializer = new XmlLayoutSerializer(dockingManager);
            using (var stream = new FileStream(layoutFileName, FileMode.Create))
            {
                serializer.Serialize(stream);
            }
        }
    }
}
