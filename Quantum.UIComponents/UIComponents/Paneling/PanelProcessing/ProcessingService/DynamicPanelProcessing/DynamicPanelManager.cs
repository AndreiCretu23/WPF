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
            LayoutAnchorablePane container = null;
            switch (Definition.GetPlacement())
            {
                case PanelPlacement.TopLeft: { container = DockingView.UpperLeftArea; break; }
                case PanelPlacement.BottomLeft: { container = DockingView.BottomLeftArea; break; }
                case PanelPlacement.Center: { container = DockingView.CenterArea; break; }
                case PanelPlacement.TopRight: { container = DockingView.UpperRightArea; break; }
                case PanelPlacement.BottomRight: { container = DockingView.BottomRightArea; break; }
                default: { throw new Exception($"Internal Error : Unregistered panel placement group position added."); }
            }

            foreach(var viewModel in ViewModelSelection.SelectedObject)
            {
                var anchorable = CreateDynamicPanel(viewModel);
                container.Children.Add(anchorable);
            }
        }
        
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
    }
}
