using System;

namespace Quantum.Services
{
    public interface ISelection
    {
        Type SelectionType { get; }
        object SelectedObject { get; }
    }
}
