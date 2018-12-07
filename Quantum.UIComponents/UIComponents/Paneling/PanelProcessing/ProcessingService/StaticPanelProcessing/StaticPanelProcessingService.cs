using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Events;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    metadata.AttachMetadataDefinition(EventAggregator, () => EventAggregator.GetEvent<StaticPanelInvalidationEvent>().Publish(new StaticPanelInvalidationArgs(definition)), ThreadOption.PublisherThread);
                }

                foreach (var metadata in definition.OfType<IBringIntoViewOnEvent>()) {
                    metadata.AttachToDefinition(EventAggregator, () =>
                    {
                        if(definition.OfType<StaticPanelConfiguration>().Single().CanOpen()) {
                            typeof(IPanelManagerService).GetMethod(nameof(PanelManager.BringStaticPanelIntoView)).MakeGenericMethod(definition.ViewModel).Invoke(PanelManager, new object[] { });
                        }
                    });
                }
            }
            VisibilityManager.SetLayoutGroupData(layoutGroupData);

        }
        
        [Handles(typeof(StaticPanelInvalidationEvent))]
        public void OnPanelInvalidation(StaticPanelInvalidationArgs args)
        {
            if (!IsUILoaded) return;

            var anchorable = anchorableDefinitions.Single(o => o.Value == args.Definition).Key;
            var config = args.Definition.OfType<StaticPanelConfiguration>().Single();

            var canOpen = config.CanOpen();
            var canClose = config.CanClose();
            if(canOpen == false && canClose == false) {
                throw new Exception($"Error invaidating static panel {args.Definition.ViewModel}. CanOpen() and CanClose() defined " +
                    $"in the panel definition's configuration metadata can't both return false at the same time.");
            }

            if(!canOpen) {
                VisibilityManager.SetVisibility(anchorable, false);
            }
            else if(!canClose) {
                VisibilityManager.SetVisibility(anchorable, true);
            }
            
            anchorable.Title = config.Title();
        }
        
        [Handles(typeof(LayoutLoadedEvent))]
        public void OnLayoutLoaded(LayoutLoadedArgs args)
        {
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
            }
        }
        
        [Handles(typeof(BringStaticPanelIntoViewRequest))]
        public void OnBringIntoViewRequest(BringStaticPanelIntoViewArgs args)
        {
            IStaticPanelDefinition definition = null;
            LayoutAnchorable anchorable = null;
            try
            {
                anchorable = anchorableDefinitions.Single(o => o.Value.ViewModel == args.PanelViewModel || o.Value.IViewModel == args.PanelViewModel).Key;
                definition = anchorableDefinitions[anchorable];
            }
            catch(InvalidOperationException)
            {
                throw new Exception($"Error : BringStaticPanelIntoView({args.PanelViewModel.Name}) : \n" +
                                    $"There is no StaticPanelDefinition registered associated with the given ViewModel type.");
            }

            if(anchorable.IsHidden && definition.CanChangeVisibility(false)) {
                VisibilityManager.SetVisibility(anchorable, true);
            }
            else
            {
                var group = anchorable.Parent.SafeCast<LayoutAnchorablePane>();
                group.SelectedContentIndex = group.Children.IndexOf(anchorable);
            }
        }
    }
}
