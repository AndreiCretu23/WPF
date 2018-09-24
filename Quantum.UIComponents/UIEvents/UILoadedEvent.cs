using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.Events
{
    /// <summary>
    /// This event is fired in the event aggregator instance of the application's container 
    /// after the MainWindow of the application is loaded.
    /// </summary>
    public class UILoadedEvent : CompositePresentationEvent<UILoadedArgs>
    {
    }

    public class UILoadedArgs
    {
    }
}
