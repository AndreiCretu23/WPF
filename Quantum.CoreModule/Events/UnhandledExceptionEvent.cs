using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.Events
{
    /// <summary>
    /// This event is raised in the EventAggregator instance of the container when an unhandled exception is thrown.
    /// </summary>
    public class UnhandledExceptionEvent : CompositePresentationEvent<UnhandledExceptionArgs>
    {
    }

    public class UnhandledExceptionArgs
    {
    }
}
