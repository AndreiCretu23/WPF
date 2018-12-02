using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Common;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal class DynamicPanelManager : ServiceBase, IDynamicPanelManager
    {
        public IDynamicPanelDefinition Definition { get; }
        public IMultipleSelection ViewModelSelection { get; }

        private IDockingView DockingView { get { return Container.Resolve<IDockingView>(); } }

        private IList<LayoutAnchorable> ActivePanels { get; } = new List<LayoutAnchorable>();

        private IList<Subscription> InvalidationSubscriptions { get; } = new List<Subscription>();

        public DynamicPanelManager(IObjectInitializationService initSvc, IDynamicPanelDefinition definition)
            : base(initSvc)
        {
            Definition = definition;
            ViewModelSelection = Definition.GetSelectionBinding(Container);
        }

        public void ProcessDefinition()
        {
            LayoutAnchorablePane container = GetDefaultLayoutContainer();
            foreach (var viewModel in ViewModelSelection.SelectedObject) {
                CreateAndAddDynamicPanel(viewModel);
            }

            EventAggregator.Subscribe(Definition.GetSelectionBindingType(), OnSelectionBindingChanged, ThreadOption.UIThread, true);
        }


        #region Events

        private void OnSelectionBindingChanged()
        {
            var activeViewModels = ActivePanels.Select(o => o.Content.SafeCast<UserControl>().DataContext);

            var addedItems = ViewModelSelection.SelectedObject.Except(activeViewModels).ToList();
            foreach (var item in addedItems) {
                CreateAndAddDynamicPanel(item);
            }

            var removedItems = activeViewModels.Except(ViewModelSelection.SelectedObject).ToList();
            foreach (var item in removedItems) {
                var anchorable = ActivePanels.Single(anch => anch.Content.SafeCast<UserControl>().DataContext == item);
                RemoveDynamicPanel(anchorable);
            }

        }

        [Handles(typeof(LayoutLoadedEvent))]
        public void OnLayoutLoaded(LayoutLoadedArgs args)
        {
            var matchingAnchorables = args.LayoutAnchorables.Where(anch => anch.Content.GetType() == Definition.View &&
                                                                           anch.Content.SafeCast<UserControl>().DataContext.GetType() == Definition.ViewModel &&
                                                                           anch.IsVisible);

            var rougeAnchorables = ActivePanels.Where(o => !matchingAnchorables.Select(anch => anch.Content.SafeCast<UserControl>().DataContext).
                                                                                Contains(o.Content.SafeCast<UserControl>().DataContext));
            foreach(var anchorable in rougeAnchorables.ToList()) {
                RemoveDynamicPanel(anchorable);
            }

            ActivePanels.Clear();
            foreach (var anchorable in matchingAnchorables) {
                ResolveDynamicPanelDependencies(anchorable);
                ActivePanels.Add(anchorable);
            }

            SyncSelection(ActivePanels.Select(o => o.Content.SafeCast<UserControl>().DataContext));
        }

        #endregion Events


        #region SelectionHandling

        private void SyncSelection(IEnumerable<object> syncWith)
        {
            using (ViewModelSelection.BeginBlockingNotifications()) {
                var addedItems = syncWith.Except(ViewModelSelection.SelectedObject).ToList();
                var removedItems = ViewModelSelection.SelectedObject.Except(syncWith).ToList();

                foreach (var item in addedItems) {
                    ViewModelSelection.Add(item);
                }
                foreach (var item in removedItems) {
                    ViewModelSelection.Remove(item);
                }
            }
        }

        #endregion SelectionHandling


        #region PanelManagement

        [SuppressMessage("Microsoft.Design", "IDE0039")]
        private void ResolveDynamicPanelDependencies(LayoutAnchorable anchorable)
        {
            var invalidationSubscriptions = ProcessInvalidators(anchorable);
            foreach (var subscription in invalidationSubscriptions) {
                InvalidationSubscriptions.Add(subscription);
            }

            InvalidateDynamicPanel(anchorable);

            EventHandler<CancelEventArgs> OnRemove = null;
            OnRemove = (sender, e) =>
            {
                var viewModel = sender.SafeCast<LayoutAnchorable>().Content.SafeCast<UserControl>().DataContext;
                ViewModelSelection.Remove(viewModel);
                if (viewModel is IInitializableObject initializable && initializable.IsInitialized) {
                    initializable.TearDown();
                }
                anchorable.Hiding -= OnRemove;
            };

            anchorable.Hiding += OnRemove;
        }

        private void AddDynamicPanel(LayoutAnchorable anchorable)
        {
            var container = GetPrefferedLayoutContainer();
            container.Children.Add(anchorable);
            container.SelectedContentIndex = container.Children.IndexOf(anchorable);

            ResolveDynamicPanelDependencies(anchorable);   

            ActivePanels.Add(anchorable);
        }

        private void CreateAndAddDynamicPanel(object viewModel)
        {
            var anchorable = CreateDynamicPanel(viewModel);
            AddDynamicPanel(anchorable);
        }

        private void RemoveDynamicPanel(LayoutAnchorable anchorable)
        {
            var invalidationSubscriptions = InvalidationSubscriptions.Where(o => o.Object == anchorable).ToList();
            foreach(var subscription in invalidationSubscriptions) {
                subscription.Event.Unsubscribe(subscription.Token);
                InvalidationSubscriptions.Remove(subscription);
            }

            var viewModel = anchorable.Content.SafeCast<UserControl>().DataContext;
            if(viewModel is IInitializableObject initializableObject && initializableObject.IsInitialized) {
                initializableObject.TearDown();
            }

            if(anchorable.GetRoot() == DockingView.DockingManager.Layout) {
                anchorable.Close();
            }

            ActivePanels.Remove(anchorable);
        }

        private void ClearDynamicPanels()
        {
            foreach(var anchorable in ActivePanels.ToList()) {
                RemoveDynamicPanel(anchorable);
            }
        }

        #endregion PanelManagement


        #region ContentOperations

        private LayoutAnchorable CreateDynamicPanel(object viewModel)
        {
            if (viewModel == null) {
                throw new Exception($"Error : $<{Definition.IView.Name}, {Definition.View.Name}, {Definition.IViewModel.Name}, {Definition.ViewModel.Name}> : \n" +
                    $"A ViewModel in the SelectionBinding is null. This is not allowed.");
            }

            if (viewModel.SafeCast<IIdentifiable>().Guid == null) {
                throw new Exception($"Error : $<{Definition.IView.Name}, {Definition.View.Name}, {Definition.IViewModel.Name}, {Definition.ViewModel.Name}> : \n" +
                                    $"A ViewModel in the SelectionBinding has a null guid. This value cannot be null because it's necessary for the serialization of the panel layout.");
            }

            var anchorable = new LayoutAnchorable();

            var view = CreateDynamicPanelView(viewModel);

            anchorable.Content = view;
            anchorable.ContentId = viewModel.SafeCast<IIdentifiable>().Guid;
            
            return anchorable;
        }

        private UserControl CreateDynamicPanelView(object viewModel)
        {
            var view = (UserControl)Activator.CreateInstance(Definition.View);
            view.DataContext = viewModel;
            return view;
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
            switch (Definition.GetPlacement()) {
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
            if (defaultContainer != null && defaultContainer.GetRoot() == DockingView.DockingManager.Layout && defaultContainer.IsVisible) {
                return defaultContainer;
            }

            var allContainers = DockingView.DockingManager.Layout.Descendents().OfType<LayoutAnchorablePane>();
            return allContainers.OrderByDescending(o => o.Children.Where(anch => anch.Content.GetType() == Definition.View &&
                                                                                 anch.Content.SafeCast<UserControl>().DataContext.GetType() == Definition.ViewModel).Count()).FirstOrDefault();

        }

        private IEnumerable<Subscription> ProcessInvalidators(LayoutAnchorable anchorable)
        {
            var invalidationTypes = Definition.OfType<AutoInvalidateOnEvent>().Select(o => o.EventType).
                             Concat(Definition.OfType<AutoInvalidateOnSelection>().Select(o => o.SelectionType));

            foreach (var invalidationType in invalidationTypes) {
                var token = EventAggregator.Subscribe(invalidationType, () => InvalidateDynamicPanel(anchorable), ThreadOption.UIThread, true);
                yield return new Subscription()
                {
                    Event = EventAggregator.GetEvent(invalidationType),
                    Object = anchorable,
                    Token = token,
                };
            }
        }

        #endregion ContentOperations


        #region BringIntoView

        public void BringPanelIntoView(object viewModel)
        {
            LayoutAnchorable anchorable = null;

            try
            {
                anchorable = ActivePanels.Single(o => o.Content.SafeCast<UserControl>().DataContext == viewModel);
            }
            catch(InvalidOperationException)
            {
                throw new Exception($"Error bringing the dynamic panel associated with the ViewModel instance of type {viewModel.GetType().Name} into view : \n " +
                                    $"The PanelSelectionBinding associated to the DynamicPanelDefinition of the given viewModelType does not contain the given instance");
            }

            var parent = anchorable.Parent.SafeCast<LayoutAnchorablePane>();
            parent.SelectedContentIndex = parent.Children.IndexOf(anchorable);
        }

        #endregion BringIntoView
    }
}
