using Microsoft.Practices.Composite.Presentation.Events;

namespace Quantum.Events
{
    /// <summary>
    /// This event is fired in the event aggregator instance of the application's container after 
    /// all the panel definitions registered in the PanelManagerService have been 
    /// processed and loaded by the framework. This happens after the UI is loaded.
    /// </summary>
    public class PanelsLoadedEvent : CompositePresentationEvent<PanelsLoadedArgs>
    {
    }

    public class PanelsLoadedArgs
    {
    }
}
