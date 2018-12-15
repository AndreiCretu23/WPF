using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Events;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal class StaticPanelProcessingService : ServiceBase, IStaticPanelProcessingService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }

        [Service]
        public IStaticPanelVisibilityManagerService VisibilityManager { get; set; }

        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }

        public StaticPanelProcessingService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        private bool IsUILoaded = false;

        [Handles(typeof(UILoadedEvent))]
        public void OnUILoaded() {
            IsUILoaded = true;
        }


        private Dictionary<LayoutAnchorable, IStaticPanelDefinition> anchorableDefinitions = new Dictionary<LayoutAnchorable, IStaticPanelDefinition>();

        public void ProcessStaticPanelDefinitions()
        {
            var layoutGroupData = new Dictionary<LayoutAnchorable, LayoutAnchorablePane>();

            foreach (var definition in PanelManager.StaticPanelDefinitions)
            {
                var config = definition.OfType<StaticPanelConfiguration>().Single();

                var anchorable = new LayoutAnchorable();

                var view = (UserControl)Activator.CreateInstance(definition.View);
                var viewModel = Container.Resolve(definition.IViewModel);
                view.DataContext = viewModel;

                anchorable.Content = view;
                anchorable.ContentId = definition.View.GetGuid();
                anchorable.Title = config.Title();

                LayoutAnchorablePane container = null;
                switch (config.Placement)
                {
                    case PanelPlacement.TopLeft: { container = DockingView.UpperLeftArea; break; }
                    case PanelPlacement.BottomLeft: { container = DockingView.BottomLeftArea; break; }
                    case PanelPlacement.Center: { container = DockingView.CenterArea; break; }
                    case PanelPlacement.TopRight: { container = DockingView.UpperRightArea; break; }
                    case PanelPlacement.BottomRight: { container = DockingView.BottomRightArea; break; }
                    default: { throw new Exception($"Internal Error : Unregistered panel placement group position added."); }
                }

                container.Children.Add(anchorable);
                layoutGroupData.Add(anchorable, container);

                anchorable.CanAutoHide = true;
                anchorable.CanFloat = config.CanFloat();

                var canOpen = config.CanOpen();
                var canClose = config.CanClose();

                if (canOpen == false && canClose == false) {
                    throw new Exception($"Error in static panel {definition.ViewModel} metadata. CanOpen() and CanClose() defined " +
                        $"in the panel definition's configuration metadata can't both return false at the same time.");
                }

                var visibility = config.IsVisible() && config.CanOpen();

                VisibilityManager.SetVisibility(anchorable, visibility);
                if (visibility)
                {
                    anchorable.CanHide = config.CanClose();
                }


                anchorableDefinitions.Add(anchorable, definition);

                anchorable.Hiding += (sender, e) => {
                    var source = sender.SafeCast<LayoutAnchorable>();
                    if (source.IsVisible)
                    {
                        VisibilityManager.UpdateContainer(source);
                    }
                };

                foreach(var metadata in definition.OfType<IAutoInvalidateMetadata>()) {
                    metadata.AttachMetadataDefinition(EventAggregator, () => InvalidateStaticPanel(definition), ThreadOption.PublisherThread);
                }

                foreach (var metadata in definition.OfType<IBringIntoViewOnEvent>()) {
                    metadata.AttachToDefinition(EventAggregator, () =>
                    {
                        if(definition.OfType<StaticPanelConfiguration>().Single().CanOpen()) {
                            BringStaticPanelIntoViewInternal(definition);
                        }
                    });
                }
            }
            VisibilityManager.SetLayoutGroupData(layoutGroupData);

        }
        
        private void InvalidateStaticPanel(IStaticPanelDefinition definition)
        {
            if (!IsUILoaded) return;

            var anchorable = anchorableDefinitions.GetKeysForValue(definition).Single();
            var config = definition.OfType<StaticPanelConfiguration>().Single();

            var canOpen = config.CanOpen();
            var canClose = config.CanClose();
            if(canOpen == false && canClose == false) {
                throw new Exception($"Error invaidating static panel {definition.ViewModel}. CanOpen() and CanClose() defined " +
                    $"in the panel definition's configuration metadata can't both return false at the same time.");
            }

            if(!canOpen && !anchorable.IsHidden) {
                VisibilityManager.SetVisibility(anchorable, false);
            }
            else if(!canClose && anchorable.IsHidden) {
                VisibilityManager.SetVisibility(anchorable, true);
            }
            
            anchorable.Title = config.Title();

            EventAggregator.GetEvent<StaticPanelInvalidationEvent>().Publish(new StaticPanelInvalidationArgs(definition));
        }
        
        [Handles(typeof(LayoutLoadedEvent))]
        public void OnLayoutLoaded(LayoutLoadedArgs args)
        {
            var layoutGroupData = new Dictionary<LayoutAnchorable, LayoutAnchorablePane>();

            var oldViews = anchorableDefinitions.Select(o => o.Key.Content as UserControl).ToList();

            anchorableDefinitions.Clear();
            var staticAnchorables = args.LayoutAnchorables.Where(o => PanelManager.StaticPanelDefinitions.Any(def => def.View == o.Content.GetType() &&
                                                                                                                     def.ViewModel == ((UserControl)o.Content).DataContext.GetType()));
            foreach (var anchorable in staticAnchorables)
            {
                var associatedDefinition = PanelManager.StaticPanelDefinitions.Single(o => o.View.GetGuid() == anchorable.Content.GetType().GetGuid() &&
                                                                                           o.ViewModel.GetGuid() == anchorable.Content.SafeCast<UserControl>().DataContext.GetType().GetGuid());
                anchorableDefinitions.Add(anchorable, associatedDefinition);
                anchorable.Hiding += (sender, e) =>
                {
                    var source = sender.SafeCast<LayoutAnchorable>();
                    VisibilityManager.UpdateContainer(source);
                };

                LayoutAnchorablePane layoutGroup = null;
                if(anchorable.Parent != null && anchorable.Parent is LayoutAnchorablePane) {
                    layoutGroup = anchorable.Parent as LayoutAnchorablePane;
                }
                else {
                    layoutGroup = anchorable.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).
                                  Single(prop => prop.Name == "PreviousContainer").GetValue(anchorable) as LayoutAnchorablePane;
                }

                layoutGroupData.Add(anchorable, layoutGroup);
            }

            // Resolve static panel definitions that were not included in the layout.
            var resolvedDefinitions = anchorableDefinitions.Select(o => o.Value);
            var newDefinitions = PanelManager.StaticPanelDefinitions.Where(o => !resolvedDefinitions.Contains(o)).ToList();
            foreach(var definition in newDefinitions) {
                var config = definition.OfType<StaticPanelConfiguration>().Single();

                var anchorable = new LayoutAnchorable();

                var view = oldViews.Single(o => o.GetType() == definition.View);
                
                anchorable.Content = view;
                anchorable.ContentId = definition.View.GetGuid();
                anchorable.Title = config.Title();
                
                LayoutAnchorablePane container = null;
                
                if (anchorableDefinitions.Any(o => o.Value.OfType<StaticPanelConfiguration>().Single().Placement == config.Placement)) {
                    container = anchorableDefinitions.Where(o => o.Value.OfType<StaticPanelConfiguration>().Single().Placement == config.Placement).Select(o => o.Key).
                                GroupBy(o => layoutGroupData[o]).OrderBy(o => o.Count()).Last().Key;
                }
                else {
                    container = DockingView.DockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();
                }

                container.Children.Add(anchorable);
                
                anchorable.CanAutoHide = true;
                anchorable.CanFloat = config.CanFloat();

                
                
                anchorable.Hiding += (sender, e) => {
                    var source = sender.SafeCast<LayoutAnchorable>();
                    if (source.IsVisible) {
                        VisibilityManager.UpdateContainer(source);
                    }
                };

                anchorableDefinitions.Add(anchorable, definition);
                layoutGroupData.Add(anchorable, container);
            }

            VisibilityManager.SetLayoutGroupData(layoutGroupData);

            foreach(var definition in newDefinitions) {
                var config = definition.OfType<StaticPanelConfiguration>().Single();

                var canOpen = config.CanOpen();
                var canClose = config.CanClose();
                var visibility = config.IsVisible();

                if (canOpen == false && canClose == false) {
                    throw new Exception($"Error in static panel {definition.ViewModel} metadata. CanOpen() and CanClose() defined " +
                        $"in the panel definition's configuration metadata can't both return false at the same time.");
                }

                if (!visibility && canClose) {
                    var anchorable = anchorableDefinitions.GetKeysForValue(definition).Single();
                    VisibilityManager.SetVisibility(anchorable, false);
                }
            }
        }
        
        [Handles(typeof(BringStaticPanelIntoViewRequest))]
        public void OnBringIntoViewRequest(BringStaticPanelIntoViewArgs args)
        {
            IStaticPanelDefinition definition = null;
            try
            {
                definition = anchorableDefinitions.Select(o => o.Value).Single(o => o.ViewModel == args.PanelViewModel || o.IViewModel == args.PanelViewModel);
            }
            catch(InvalidOperationException)
            {
                throw new Exception($"Error : BringStaticPanelIntoView({args.PanelViewModel.Name}) : \n" +
                                    $"There is no StaticPanelDefinition registered associated with the given ViewModel type.");
            }

            BringStaticPanelIntoViewInternal(definition);
        }


        private void BringStaticPanelIntoViewInternal(IStaticPanelDefinition definition)
        {
            definition.AssertParameterNotNull(nameof(definition));
            LayoutAnchorable anchorable = null;
            try 
            {
                anchorable = anchorableDefinitions.GetKeysForValue(definition).Single();
            }
            catch(InvalidOperationException) 
            {
                throw new Exception($"Error : Cannot find a unique registered anchorable associated with the static panel definition " +
                                    $"{definition.IView},{definition.View}, {definition.IViewModel}, {definition.ViewModel}");
            }

            if (anchorable.IsHidden && definition.CanChangeVisibility(false)) {
                VisibilityManager.SetVisibility(anchorable, true);
            }
            else {
                var group = anchorable.Parent.SafeCast<LayoutAnchorablePane>();
                group.SelectedContentIndex = group.Children.IndexOf(anchorable);
            }
        }

    }
}
