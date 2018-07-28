using Microsoft.Practices.Composite.Presentation.Events;
using System;

namespace Quantum.UIComponents
{
    internal class ToolBarLayoutChangedEvent : CompositePresentationEvent<ToolBarLayoutChangedArgs>
    {
    }

    internal class ToolBarLayoutChangedArgs
    {
        public Type View { get; set; }
        public Type ViewModel { get; set; }

        public int Band { get; set; }
        public int BandIndex { get; set; }

        public ToolBarLayoutChangedArgs()
        {
        }
    }
}
