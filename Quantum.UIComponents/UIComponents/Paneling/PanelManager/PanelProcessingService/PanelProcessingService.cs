using System;
using System.Collections.Generic;
using System.IO;
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
        public IPanelVisibilityManagerService VisibilityManager { get; set; }

        [Service]
        public IPanelLayoutManagerService LayoutManager { get; set; }

        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }
        
        public PanelProcessingService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            ProcessConfig();
        }

        private bool IsUILoaded = false;

        [Handles(typeof(UILoadedEvent))]
        public void OnUILoaded()
        {
            IsUILoaded = true;
            ProcessStaticPanelDefinitions();
            ProcessDynamicPanelDefinitions();
            EventAggregator.GetEvent<PanelsLoadedEvent>().Publish(new PanelsLoadedArgs());
        }

        #region StaticPanelProcessing

        private Dictionary<LayoutAnchorable, IStaticPanelDefinition> anchorableDefinitions = new Dictionary<LayoutAnchorable, IStaticPanelDefinition>();

        private void ProcessStaticPanelDefinitions()
        {
            var layoutGroupData = new Dictionary<LayoutAnchorable, LayoutAnchorablePane>();

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

                var visibility = config.IsVisible() && config.CanOpen();
                
                VisibilityManager.SetVisibility(anchorable, visibility);
                if (visibility) {
                    anchorable.CanHide = config.CanClose();
                }
                

                anchorableDefinitions.Add(anchorable, definition);

                anchorable.Hiding += (sender, e) => {
                    if(anchorable.IsVisible)
                    {
                        EventAggregator.GetEvent<PanelVisibilityChangedEvent>().Publish(new PanelVisibilityChangedArgs(definition, false));
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
            VisibilityManager.SetLayoutGroupData(layoutGroupData);
            
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
                VisibilityManager.SetVisibility(anchorable, true);
                anchorable.CanHide = config.CanClose();
            }
            else if(!visibility && config.CanClose() && anchorable.IsVisible)
            {
                VisibilityManager.SetVisibility(anchorable, false);
            }
        }

        [Handles(typeof(PanelMenuEntryStateChangedEvent))]
        public void OnPanelVisibilityChanged(PanelMenuEntryStateChangedArgs args)
        {
            if (!IsUILoaded) return;

            var anchorable = anchorableDefinitions.Single(o => o.Value == args.Definition).Key;
            
            VisibilityManager.SetVisibility(anchorable, args.Visibility);
            if(args.Visibility)
            {
                anchorable.CanHide = anchorableDefinitions[anchorable].OfType<StaticPanelConfiguration>().Single().CanClose();
            }
        }

        #endregion StaticPanelProcessing

        private void ProcessDynamicPanelDefinitions()
        {

        }

        #region ConfigProcessing

        private void ProcessConfig()
        {
            var config = PanelManager.DockingConfiguration;
            var configDirectory = config.LayoutSerializationDirectory;
            if(!Directory.Exists(configDirectory))
            {
                throw new DirectoryNotFoundException($"Error : Panel Configuration Directory {configDirectory} not found!");
            }

            var layoutDirectory = Path.Combine(configDirectory, "Layout");
            if(!Directory.Exists(layoutDirectory)) {
                Directory.CreateDirectory(layoutDirectory);
            }

            EventAggregator.Subscribe(config.LayoutSerializationEvent, () => LayoutManager.SaveLayout(layoutDirectory));
            EventAggregator.Subscribe(config.LayoutDeserializationEvent, () =>
            {
                if(File.Exists(Path.Combine(layoutDirectory, "PanelLayout.xml")))
                {
                    LayoutManager.LoadLayout(layoutDirectory);
                }
            });

        }

        #endregion ConfigProcessing


        #region OnLayoutLoaded

        [Handles(typeof(LayoutLoadedEvent))]
        public void OnLayoutLoaded(LayoutLoadedArgs args)
        {
            anchorableDefinitions.Clear();
            foreach (var anchorable in args.LayoutAnchorables)
            {
                var associatedDefinition = PanelManager.StaticPanelDefinitions.Single(o => o.View.GetGuid() == anchorable.Content.GetType().GetGuid() &&
                                                                                           o.ViewModel.GetGuid() == anchorable.Content.SafeCast<UserControl>().DataContext.GetType().GetGuid());
                anchorableDefinitions.Add(anchorable, associatedDefinition);
                anchorable.Hiding += (sender, e) => EventAggregator.GetEvent<PanelVisibilityChangedEvent>().Publish(new PanelVisibilityChangedArgs(anchorableDefinitions[anchorable], false));
            }
        }

        #endregion OnLayoutLoaded

    }
}
