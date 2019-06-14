using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Command;
using Quantum.UIComponents;

namespace Quantum.Events
{
    internal class ShortcutChangedEvent : CompositePresentationEvent<IShortcutChangedArgs>
    {
    }

    internal interface IShortcutChangedArgs
    {
    }

    internal class GlobalRebuildShortcutChangedArgs : IShortcutChangedArgs
    {
    }

    internal class GlobalCommandShortcutChangedArgs : IShortcutChangedArgs
    {
        public IGlobalCommand Command { get; }
        public GlobalCommandShortcutChangedArgs(IGlobalCommand command)
        {
            Command = command;
        }
    }

    internal class BringPanelIntoViewShortcutChangedArgs : IShortcutChangedArgs
    {
        public IStaticPanelDefinition PanelDefinition { get; }
        public BringPanelIntoViewShortcutChangedArgs(IStaticPanelDefinition panelDefinition)
        {
            PanelDefinition = panelDefinition;
        }
    }
}
