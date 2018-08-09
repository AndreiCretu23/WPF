using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Events;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal class PanelProcessingService : QuantumServiceBase, IPanelProcessingService
    {
        [Service]
        public IPanelManagerService PanelManager { get; set; }
        
        [Service]
        public IPanelLayoutManagerService LayoutManager { get; set; }
        
        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }
        
        public PanelProcessingService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        private bool IsUILoaded = false;

        [Handles(typeof(UILoadedEvent))]
        public void OnUILoaded()
        {
            IsUILoaded = true;
            ProcessStaticPanelDefinitions();
            ProcessDynamicPanelDefinitions();
        }

        #region StaticPanelProcessing

        private Dictionary<LayoutAnchorable, IStaticPanelDefinition> anchorableDefinitions = new Dictionary<LayoutAnchorable, IStaticPanelDefinition>();

        private void ProcessStaticPanelDefinitions()
        {
            foreach(var definition in PanelManager.StaticPanelDefinitions)
            {
                var config = definition.OfType<StaticPanelConfiguration>().Single();

                var anchorable = new LayoutAnchorable();
                
                var view = (UserControl)Activator.CreateInstance(definition.View);
                var viewModel = Container.Resolve(definition.IViewModel);
                view.DataContext = viewModel;

                anchorable.Content = view;
                anchorable.ContentId = definition.View.GetGuid();
                anchorable.Title = config.Title();

                switch (config.Placement)
                {
                    case PanelPlacement.TopLeft: { DockingView.UpperLeftArea.Children.Add(anchorable); break; }
                    case PanelPlacement.BottomLeft: { DockingView.BottomLeftArea.Children.Add(anchorable); break; }
                    case PanelPlacement.Center: { DockingView.CenterArea.Children.Add(anchorable); break; }
                    case PanelPlacement.TopRight: { DockingView.UpperRightArea.Children.Add(anchorable); break; }
                    case PanelPlacement.BottomRight: { DockingView.BottomRightArea.Children.Add(anchorable); break; }
                    default: { throw new Exception($"Internal Error : Unregistered panel placement group position added."); }
                }

                anchorable.CanAutoHide = true;
                anchorable.CanFloat = config.CanFloat();
                anchorable.Show();

                var visibility = config.IsVisible() && config.CanOpen();

                anchorable.SetVisibility(visibility);
                if (visibility) {
                    anchorable.CanClose = config.CanClose();
                }

                anchorableDefinitions.Add(anchorable, definition);

                anchorable.Hiding += (sender, e) => {
                    if(anchorable.IsVisible)
                    {
                        EventAggregator.GetEvent<PanelClosingEvent>().Publish(new PanelClosingArgs(definition));
                    }
                    
                };


                var invalidationTypes = definition.OfType<AutoInvalidateOnEvent>().Select(o => o.EventType).
                                 Concat(definition.OfType<AutoInvalidateOnSelection>().Select(o => o.SelectionType));

                foreach (var invalidationEvent in invalidationTypes)
                {
                    EventAggregator.Subscribe(invalidationEvent, () =>
                    {
                        EventAggregator.GetEvent<PanelInvalidationEvent>().Publish(new PanelInvalidationArgs(definition));
                    }, ThreadOption.PublisherThread, true);
                }
            }
        }

        [Handles(typeof(PanelInvalidationEvent))]
        public void OnPanelInvalidation(PanelInvalidationArgs args)
        {
            if (!IsUILoaded) return;

            var anchorable = anchorableDefinitions.Single(o => o.Value == args.Definition).Key;
            var config = args.Definition.OfType<StaticPanelConfiguration>().Single();

            anchorable.Title = config.Title();

            var visibility = config.IsVisible();
            if(visibility && config.CanOpen() && !anchorable.IsVisible)
            {
                anchorable.SetVisibility(true);
                anchorable.CanClose = config.CanClose();
            }
            else if(!visibility && config.CanClose() && anchorable.IsVisible)
            {
                anchorable.SetVisibility(false);
            }

            //EventAggregator.GetEvent<PanelVisibilityChangedEvent>().Publish(new PanelVisibilityChangedArgs(args.Definition.IViewModel, anchorable.IsVisible));
        }

        [Handles(typeof(PanelMenuEntryStateChangedEvent))]
        public void OnPanelVisibilityChanged(PanelMenuEntryStateChangedArgs args)
        {
            if (!IsUILoaded) return;

            var anchorable = anchorableDefinitions.Single(o => o.Value == args.Definition).Key;
            //if(anchorable.IsVisible != args.Visibility)
            //{
                anchorable.SetVisibility(args.Visibility);
                if(args.Visibility)
                {
                    anchorable.CanClose = anchorableDefinitions[anchorable].OfType<StaticPanelConfiguration>().Single().CanClose();
                }
            //}
        }

        #endregion StaticPanelProcessing

        private void ProcessDynamicPanelDefinitions()
        {

        }

    }
}
