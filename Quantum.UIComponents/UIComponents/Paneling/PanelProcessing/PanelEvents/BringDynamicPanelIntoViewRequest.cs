using Microsoft.Practices.Composite.Presentation.Events;
using System;

namespace Quantum.UIComponents
{
    internal class BringDynamicPanelIntoViewRequest : CompositePresentationEvent<BringDynamicPanelIntoViewArgs>
    {
    }

    internal class BringDynamicPanelIntoViewArgs
    {
        public Type PanelViewModel { get; private set; }
        public object ViewModel { get; private set; }

        public BringDynamicPanelIntoViewArgs(Type panelViewModel, object viewModel)
        {
            PanelViewModel = panelViewModel;
            ViewModel = viewModel;
        }

    }
}
