using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.UIComponents
{
    internal class PanelInvalidationEvent : CompositePresentationEvent<PanelInvalidationArgs>
    {
    }

    internal class PanelInvalidationArgs
    {
        internal IStaticPanelDefinition Definition { get; private set; }

        internal PanelInvalidationArgs(IStaticPanelDefinition definition)
        {
            Definition = definition;
        }
    }
}
