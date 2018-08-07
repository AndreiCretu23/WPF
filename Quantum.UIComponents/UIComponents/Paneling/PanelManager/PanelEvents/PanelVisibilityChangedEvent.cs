using Microsoft.Practices.Composite.Presentation.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    internal class PanelVisibilityChangedEvent : CompositePresentationEvent<PanelVisibilityChangedArgs>
    {
    }

    internal class PanelVisibilityChangedArgs
    {
        public Type IViewModel { get; private set; }
        public bool Visibility { get; private set; }

        public PanelVisibilityChangedArgs(Type iViewModel, bool visibility)
        {
            IViewModel = iViewModel;
            Visibility = visibility;
        }
    }
}
