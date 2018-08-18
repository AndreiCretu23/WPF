using Microsoft.Practices.Composite.Presentation.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal class LayoutLoadedEvent : CompositePresentationEvent<LayoutLoadedArgs>
    {
    }

    internal class LayoutLoadedArgs
    {
        public IEnumerable<LayoutAnchorable> LayoutAnchorables { get; private set; }

        public LayoutLoadedArgs(IEnumerable<LayoutAnchorable> anchorables)
        {
            LayoutAnchorables = anchorables;
        }
    }
}
