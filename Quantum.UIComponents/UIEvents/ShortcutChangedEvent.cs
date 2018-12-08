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

    internal class ManagedCommandShortcutChangedArgs : IShortcutChangedArgs
    {
        public IManagedCommand Command { get; }
        public ManagedCommandShortcutChangedArgs(IManagedCommand command)
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
