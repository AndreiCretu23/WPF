namespace Quantum.Services
{
    /// <summary>
    /// Provides a basic contract for any SingleSelection.
    /// </summary>
    public interface ISingleSelection : ISelection
    {
        /// <summary>
        /// A wrapper that stores the previously selected item. When the value of the selection is set, 
        /// a new wrapper instance is created containing the value of the previously selected item. After that, the 
        /// event is raised and handled by the application. Finally, the wrapper is disposed. <para></para>
        /// WARNING : If a subscriber handles the selection changing on a separate thread, the event raising process will just trigger the handler, 
        ///           and it will not wait for it to finish, meaning the OldValue will get disposed immediately, and will not be accessible inside the handler. 
        /// </summary>
        ISingleSelectionCache OldValue { get; }
    }
}
