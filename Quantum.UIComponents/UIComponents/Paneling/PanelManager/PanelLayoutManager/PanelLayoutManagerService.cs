using Quantum.Services;
using Quantum.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Quantum.UIComponents
{
    internal class PanelLayoutManagerService : QuantumServiceBase, IPanelLayoutManagerService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }

        [Service]
        public IPanelVisibilityManagerService VisibilityManager { get; set; }

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
                
                var layoutElements = DockingView.DockingManager.Layout.Descendents();

                var anchorables = layoutElements.OfType<LayoutAnchorable>();
                var groups = layoutElements.OfType<LayoutAnchorablePane>();

                var layoutGroupData = new Dictionary<LayoutAnchorable, LayoutAnchorablePane>();
                foreach (var anchorable in anchorables)
                {
                    var viewGuid = anchorable.Content.GetType().GetGuid();
                    var viewModelGuid = anchorable.Content.SafeCast<UserControl>().DataContext.GetType().GetGuid();
                    
                    LayoutAnchorablePane layoutGroup = null;

                    if (anchorable.Parent != null && anchorable.Parent is LayoutAnchorablePane)
                    {
                        layoutGroup = anchorable.Parent as LayoutAnchorablePane;
                    }
                    else
                    {
                        layoutGroup = anchorable.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).
                                      Single(prop => prop.Name == "PreviousContainer").GetValue(anchorable) as LayoutAnchorablePane;
                    }
                    
                    layoutGroupData.Add(anchorable, layoutGroup);
                    EventAggregator.GetEvent<PanelVisibilityChangedEvent>().Publish(new PanelVisibilityChangedArgs(PanelManager.StaticPanelDefinitions.Single(o => o.View.GetGuid() == viewGuid && o.ViewModel.GetGuid() == viewModelGuid), !anchorable.IsHidden));
                }

                VisibilityManager.SetLayoutGroupData(layoutGroupData);
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
