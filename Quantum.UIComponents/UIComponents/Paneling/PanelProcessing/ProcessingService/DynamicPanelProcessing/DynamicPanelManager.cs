using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quantum.Services;
using Quantum.Metadata;
using Xceed.Wpf.AvalonDock.Layout;
using Quantum.Utils;
using System.Windows.Controls;
using Quantum.Common;
using Microsoft.Practices.Composite.Presentation.Events;
using System.Collections.ObjectModel;

namespace Quantum.UIComponents
{
    internal class DynamicPanelManager : QuantumServiceBase, IDynamicPanelManager
    {
        public IDynamicPanelDefinition Definition { get; private set; }
        public IMultipleSelection ViewModelSelection { get; private set; }

        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }

        private List<LayoutAnchorable> ActivePanels { get; set; } = new List<LayoutAnchorable>();



        public DynamicPanelManager(IObjectInitializationService initSvc, IDynamicPanelDefinition definition)
            : base(initSvc)
        {
            Definition = definition;
            ViewModelSelection = Definition.GetSelectionBinding(Container);
        }
        
        public void ProcessDefinition()
        {
            LayoutAnchorablePane container = GetDefaultLayoutContainer();

            foreach(var viewModel in ViewModelSelection.SelectedObject)
            {
                var anchorable = CreateDynamicPanel(viewModel);
                container.Children.Add(anchorable);
                ActivePanels.Add(anchorable);
            }

            EventAggregator.Subscribe(Definition.GetSelectionBindingType(), OnSelectionBindingChanged, ThreadOption.UIThread, true);
        }

        #region Events

        private void OnSelectionBindingChanged()
        {
            var activeViewModels = ActivePanels.Select(o => o.Content.SafeCast<UserControl>().DataContext);

            var addedItems = ViewModelSelection.SelectedObject.Except(activeViewModels).ToList();
            foreach(var item in addedItems) {
                var anchorable = CreateDynamicPanel(item);
                var group = GetPrefferedLayoutContainer();
                group.Children.Add(anchorable);
                group.SelectedContentIndex = group.Children.IndexOf(anchorable);
                ActivePanels.Add(anchorable);
            }

            var removedItems = activeViewModels.Except(ViewModelSelection.SelectedObject).ToList();
            foreach(var item in removedItems)
            {
                var anchorable = ActivePanels.Single(anch => anch.Content.SafeCast<UserControl>().DataContext == item);
                anchorable.Close();
                ActivePanels.Remove(anchorable);
            }
            
        }

        [Handles(typeof(LayoutLoadedEvent))]
        public void OnLayoutLoaded(LayoutLoadedArgs args)
        {
            var matchingAnchorables = args.LayoutAnchorables.Where(anch => anch.Content.GetType() == Definition.View && 
                                                                           anch.Content.SafeCast<UserControl>().DataContext.GetType() == Definition.ViewModel && 
                                                                           anch.IsVisible);

            ActivePanels.Clear();
            foreach(var anchorable in matchingAnchorables)
            {
                ActivePanels.Add(anchorable);
            }

            SyncSelection(ActivePanels.Select(o => o.Content.SafeCast<UserControl>().DataContext));
        }

        #endregion Events


        #region SelectionHandling

        private void SyncSelection(IEnumerable<object> syncWith)
        {
            using (ViewModelSelection.BeginBlockingNotifications())
            {
                var addedItems = syncWith.Except(ViewModelSelection.SelectedObject).ToList();
                var removedItems = ViewModelSelection.SelectedObject.Except(syncWith).ToList();

                foreach(var item in addedItems) {
                    ViewModelSelection.Add(item);
                }
                foreach(var item in removedItems) {
                    ViewModelSelection.Remove(item);
                }
            }
        }

        #endregion SelectionHandling


        #region ContentOperations

        private LayoutAnchorable CreateDynamicPanel(object viewModel)
        {
            if (viewModel == null)
            {
                throw new Exception($"Error : $<{Definition.IView.Name}, {Definition.View.Name}, {Definition.IViewModel.Name}, {Definition.ViewModel.Name}> : \n" +
                    $"A ViewModel in the SelectionBinding is null. This is not allowed.");
            }

            if(viewModel.SafeCast<IIdentifiable>().Guid == null)
            {
                throw new Exception($"Error : $<{Definition.IView.Name}, {Definition.View.Name}, {Definition.IViewModel.Name}, {Definition.ViewModel.Name}> : \n" +
                                    $"A ViewModel in the SelectionBinding has a null guid. This value cannot be null because it's necessary for the serialization of the panel layout.");
            }

            var anchorable = new LayoutAnchorable();

            var view = CreateDynamicPanelView(viewModel);

            anchorable.Content = view;
            InvalidateDynamicPanel(anchorable);
            anchorable.ContentId = viewModel.SafeCast<IIdentifiable>().Guid;

            ProcessInvalidators(anchorable);

            anchorable.Hiding += (sender, e) =>
            {
                ViewModelSelection.Remove(sender.SafeCast<LayoutAnchorable>().Content.SafeCast<UserControl>().DataContext);
            };

            return anchorable;
        }

        private UserControl CreateDynamicPanelView(object viewModel)
        {
            var view = (UserControl)Activator.CreateInstance(Definition.View);
            view.DataContext = viewModel;
            return view;
        }

        private void ProcessInvalidators(LayoutAnchorable anchorable)
        {
            var invalidationTypes = Definition.OfType<AutoInvalidateOnEvent>().Select(o => o.EventType).
                             Concat(Definition.OfType<AutoInvalidateOnSelection>().Select(o => o.SelectionType));

            foreach(var invalidationType in invalidationTypes)
            {
                EventAggregator.Subscribe(invalidationType, () => InvalidateDynamicPanel(anchorable), ThreadOption.UIThread, true);
            }
        }

        private void InvalidateDynamicPanel(LayoutAnchorable anchorable)
        {
            var view = (UserControl)anchorable.Content;
            var viewModel = view.DataContext;

            anchorable.Title = Definition.ComputeTitle(view, viewModel);
            anchorable.CanFloat = Definition.ComputeCanFloat(view, viewModel);
        }

        private LayoutAnchorablePane GetDefaultLayoutContainer()
        {
            switch (Definition.GetPlacement())
            {
                case PanelPlacement.TopLeft: { return DockingView.UpperLeftArea; }
                case PanelPlacement.BottomLeft: { return DockingView.BottomLeftArea; }
                case PanelPlacement.Center: { return DockingView.CenterArea; }
                case PanelPlacement.TopRight: { return DockingView.UpperRightArea; }
                case PanelPlacement.BottomRight: { return DockingView.BottomRightArea; }
                default: { throw new Exception($"Internal Error : Unregistered panel placement group position added."); }
            }
        }

        private LayoutAnchorablePane GetPrefferedLayoutContainer()
        {
            var defaultContainer = GetDefaultLayoutContainer();
            if(defaultContainer != null && defaultContainer.GetRoot() == DockingView.DockingManager.Layout && defaultContainer.IsVisible) {
                return defaultContainer;
            }

            var allContainers = DockingView.DockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>();
            return allContainers.OrderByDescending(o => o.Children.Where(anch => anch.Content.GetType() == Definition.View &&
                                                                                 anch.Content.SafeCast<UserControl>().DataContext.GetType() == Definition.ViewModel).Count()).FirstOrDefault();
            
        }

        #endregion ContentOperations
    }
}
