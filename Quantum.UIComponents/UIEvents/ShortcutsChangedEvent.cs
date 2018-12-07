using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Command;

namespace Quantum.UIComponents.UIEvents
{
    internal class ShortcutsChangedEvent : CompositePresentationEvent<ShortcutsChangedArgs>
    {
    }

    internal class ShortcutsChangedArgs
    {
        public IUICommand Command { get; }

        public ShortcutsChangedArgs(IUICommand command)
        {
            Command = command;
        }
    }
}
