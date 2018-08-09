using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.UIComponents
{
    internal class PanelClosingEvent : CompositePresentationEvent<PanelClosingArgs>
    {
    }

    internal class PanelClosingArgs
    {
        public IStaticPanelDefinition Definition { get; private set; }
        
        public PanelClosingArgs(IStaticPanelDefinition definition)
        {
            Definition = definition;
        }
    }
}
