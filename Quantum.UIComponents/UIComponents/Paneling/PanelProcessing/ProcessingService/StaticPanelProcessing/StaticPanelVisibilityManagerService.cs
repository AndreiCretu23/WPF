using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Quantum.Services;
using Quantum.Utils;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal class StaticPanelVisibilityManagerService : QuantumServiceBase, IStaticPanelVisibilityManagerService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }

        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }

        private Dictionary<LayoutAnchorable, LayoutAnchorablePane> LayoutGroups = new Dictionary<LayoutAnchorable, LayoutAnchorablePane>();

        public StaticPanelVisibilityManagerService(IObjectInitializationService initSvc)
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

        [Handles(typeof(LayoutLoadedEvent))]
        public void OnLayoutLoaded(LayoutLoadedArgs args)
        {
            var layoutElements = DockingView.DockingManager.Layout.Descendents();

            var staticAnchorables = args.LayoutAnchorables.Where(o => PanelManager.StaticPanelDefinitions.Any(def => def.View == o.Content.GetType() && 
                                                                                                                     def.ViewModel == ((UserControl)o.Content).DataContext.GetType()));
            var groups = layoutElements.OfType<LayoutAnchorablePane>();

            var layoutGroupData = new Dictionary<LayoutAnchorable, LayoutAnchorablePane>();
            foreach (var anchorable in staticAnchorables)
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
                EventAggregator.GetEvent<StaticPanelVisibilityChangedEvent>().Publish(new StaticPanelVisibilityChangedArgs(PanelManager.StaticPanelDefinitions.Single(o => o.View.GetGuid() == viewGuid && o.ViewModel.GetGuid() == viewModelGuid), !anchorable.IsHidden));
            }

            SetLayoutGroupData(layoutGroupData);
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
                    LayoutGroups[anchorable].SelectedContentIndex = LayoutGroups[anchorable].Children.IndexOf(anchorable);
                }
                else
                {
                    UpdateContainer(anchorable);
                    anchorable.Hide();
                }
            }
            catch
            {
                //Do nothing. Means the layout anchorable has not been loaded yet in the UI.
            }
        }

        public void UpdateContainer(LayoutAnchorable anchorable)
        {
            if(anchorable.Parent != null && anchorable.Parent is LayoutAnchorablePane)
            {
                LayoutGroups[anchorable] = anchorable.Parent as LayoutAnchorablePane;
            }
        }

        #endregion VisibilitySetter
    }
}
