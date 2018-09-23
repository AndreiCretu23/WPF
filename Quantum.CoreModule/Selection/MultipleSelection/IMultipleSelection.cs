using System;
using System.Collections.Generic;

namespace Quantum.Services
{
    /// <summary>
    /// Provides a basic contract for any MultipleSelection.
    /// </summary>
    public interface IMultipleSelection : ISelection
    {
        /// <summary>
        /// Returns the type of the objects in the selection.
        /// </summary>
        new Type SelectionType { get; }

        /// <summary>
        /// Returns an enumeration containing all values currently in the selection.
        /// </summary>
        new IEnumerable<object> SelectedObject { get; }

        /// <summary>
        /// A wrapper that stores the the added and removed items of the current selection changing scope. When items are added or removed from the selection, 
        /// a new wrapper instance is created in which the newly added and removed items are stored. If the add/remove operations are done within a 
        /// notification blocking scope, then the changed will stack up inside the cache instance. Else the added/removed item will be added in the corresponding
        /// collection of the cache. Either after the added/removed item is added in the cache(not using a blocking notification scope), or the selection 
        /// blocking notification scope ends, the event will be raised and handled by the application. Finally, the wrapper is disposed.<para></para>
        /// WARNING : If a subscriber handles the selection changing on a separate thread, the event raising process will just trigger the handler, 
        ///           and it will not wait for it to finish, meaning the ChangedItem cache will get disposed immediately, and will not be accessible inside the handler. 
        /// </summary>
        IMultipleSelectionCache ChangedItems { get; }

        /// <summary>
        /// Adds the given value to the selection collection and notifies the listeners that the selection has changed. 
        /// If the type of the given value does not match the type of the current selection instance, an exception will be thrown.
        /// </summary>
        /// <param name="value"></param>
        void Add(object value);

        /// <summary>
        /// Attempts to remove the specified value from the selection, and if successful, it nofifies the listeners that the selection has changed.
        /// If the type of the given value does not match the type of the current selection instance, an exception will be thrown.
        /// </summary>
        /// <param name="value"></param>
        void Remove(object value);
    }
}
