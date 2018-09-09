using Quantum.Services;
using Quantum.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal class StaticPanelVisibilityManagerService : ServiceBase, IStaticPanelVisibilityManagerService
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
                // Due to serializing/deserializing the application multiple times, a reference an an anchorable/an anchorable's parent might be lost.
                // If that's the case, we refresh the view and add it in the first available pane.
                if(DockingView != null && DockingView.DockingManager != null)
                {
                    var root = DockingView.DockingManager.Layout;
                    var availablePane = root.IfNotNull(o => o.Descendents().ExcludeDefaultValues().OfType<LayoutAnchorablePane>().FirstOrDefault());
                    if(availablePane != null)
                    {
                        if(LayoutGroups.ContainsKey(anchorable))
                        {
                            LayoutGroups[anchorable] = availablePane;
                        }
                        else
                        {
                            LayoutGroups.Add(anchorable, availablePane);
                        }
                        LayoutGroups[anchorable].Children.Add(anchorable);
                        LayoutGroups[anchorable].SelectedContentIndex = LayoutGroups[anchorable].Children.IndexOf(anchorable);
                    }
                }

                // Do nothing. If this point is reached, it means the layout anchorable has not been loaded yet in the UI.
                // This can happen due to various UI events triggering by the initialization of viewModel components(MainMenuViewModel for example).
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
