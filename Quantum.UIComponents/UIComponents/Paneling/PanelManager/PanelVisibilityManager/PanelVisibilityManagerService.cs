using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Services;
using Quantum.Utils;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal class PanelVisibilityManagerService : QuantumServiceBase, IPanelVisibilityManagerService
    {
        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }

        private Dictionary<LayoutAnchorable, LayoutAnchorablePane> LayoutGroups = new Dictionary<LayoutAnchorable, LayoutAnchorablePane>();

        public PanelVisibilityManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        #region DataManagement
        
        public void SetLayoutGroupData(IDictionary<LayoutAnchorable, LayoutAnchorablePane> layoutData)
        {
            LayoutGroups.Clear();
            foreach(var data in layoutData)
            {
                LayoutGroups.Add(data.Key, data.Value);
            }
        }

        #endregion DataManagement

        #region VisibilitySetter

        public void SetVisibility(LayoutAnchorable anchorable, bool visibility)
        {
            anchorable.AssertNotNull(nameof(anchorable));
            try
            {
                if (visibility)
                {
                    LayoutGroups[anchorable].Children.Add(anchorable);
                    anchorable.Show();
                    var vis = LayoutGroups[anchorable].IsVisible;
                }
                else
                {
                    if (anchorable.Parent != null && anchorable.Parent is LayoutAnchorablePane)
                    {
                        LayoutGroups[anchorable] = anchorable.Parent as LayoutAnchorablePane;
                    }
                    anchorable.Hide();
                }
            }
            catch
            {
                //Do nothing. Means the layout anchorable has not been loaded yet in the UI.
            }
        }

        #endregion VisibilitySetter
    }
}
