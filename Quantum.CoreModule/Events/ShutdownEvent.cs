using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.Events
{
    /// <summary>
    /// This event is raised in the EventAggregator instance of the container when the application shuts down.
    /// </summary>
    public class ShutdownEvent : CompositePresentationEvent<ShutdownArgs>
    {
    }

    public class ShutdownArgs
    {
    }
}
