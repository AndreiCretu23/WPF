using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.UIComponents
{
    internal class PanelMenuEntryStateChangedEvent : CompositePresentationEvent<PanelMenuEntryStateChangedArgs>
    {
    }

    internal class PanelMenuEntryStateChangedArgs
    {
        public IStaticPanelDefinition Definition { get; private set; }
        public bool Visibility { get; private set; }

        public PanelMenuEntryStateChangedArgs(IStaticPanelDefinition definition, bool visibility)
        {
            Definition = definition;
            Visibility = visibility;
        }
    }

}
