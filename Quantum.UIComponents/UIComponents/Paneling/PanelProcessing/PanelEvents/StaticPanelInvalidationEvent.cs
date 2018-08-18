using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.UIComponents
{
    internal class StaticPanelInvalidationEvent : CompositePresentationEvent<StaticPanelInvalidationArgs>
    {
    }

    internal class StaticPanelInvalidationArgs
    {
        internal IStaticPanelDefinition Definition { get; private set; }

        internal StaticPanelInvalidationArgs(IStaticPanelDefinition definition)
        {
            Definition = definition;
        }
    }
}
