using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.UIComponents
{
    internal class PanelVisibilityChangedEvent : CompositePresentationEvent<PanelVisibilityChangedArgs>
    {
    }

    internal class PanelVisibilityChangedArgs
    {
        public IStaticPanelDefinition Definition { get; private set; }
        public bool Visibility { get; private set; }

        public PanelVisibilityChangedArgs(IStaticPanelDefinition definition, bool visibility)
        {
            Definition = definition;
            Visibility = visibility;
        }
    }
}
