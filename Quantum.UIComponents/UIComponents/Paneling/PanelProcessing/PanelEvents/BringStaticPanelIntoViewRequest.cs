using Microsoft.Practices.Composite.Presentation.Events;
using System;

namespace Quantum.UIComponents
{
    internal class BringStaticPanelIntoViewRequest : CompositePresentationEvent<BringStaticPanelIntoViewArgs>
    {
    }

    internal class BringStaticPanelIntoViewArgs
    {
        public Type PanelViewModel { get; private set; }

        public BringStaticPanelIntoViewArgs(Type panelViewModel)
        {
            PanelViewModel = panelViewModel;
        }
    }
}
