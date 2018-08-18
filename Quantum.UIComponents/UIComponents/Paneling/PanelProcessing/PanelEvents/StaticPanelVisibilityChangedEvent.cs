using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.UIComponents
{
    internal class StaticPanelVisibilityChangedEvent : CompositePresentationEvent<StaticPanelVisibilityChangedArgs>
    {
    }

    internal class StaticPanelVisibilityChangedArgs
    {
        public IStaticPanelDefinition Definition { get; private set; }
        public bool Visibility { get; private set; }

        public StaticPanelVisibilityChangedArgs(IStaticPanelDefinition definition, bool visibility)
        {
            Definition = definition;
            Visibility = visibility;
        }
    }
}
