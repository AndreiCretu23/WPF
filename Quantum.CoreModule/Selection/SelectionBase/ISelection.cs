using System;

namespace Quantum.Services
{
    public interface ISelection
    {
        IDisposable BeginBlockingNotifications();

        Type SelectionType { get; }
        object SelectedObject { get; }
    }
}
