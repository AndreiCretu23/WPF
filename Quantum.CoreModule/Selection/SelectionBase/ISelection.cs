using System;

namespace Quantum.Services
{
    /// <summary>
    /// Provides a basic contract for any selection.
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Begins a scope in which all notifications are disabled. If the value changes inside this scope, 
        /// the event will not raise itself. When the scope ends, if the value changed during it, the event will raise itself.
        /// </summary>
        /// <returns></returns>
        IDisposable BeginBlockingNotifications();

        /// <summary>
        /// Returns the type of the object stored in the selection.
        /// </summary>
        Type SelectionType { get; }

        /// <summary>
        /// Returns the value currently stored inside the instance of the selection.
        /// </summary>
        object SelectedObject { get; }
    }
}
