using System;
using System.Collections.Generic;

namespace Quantum.Services
{
    public interface IMultipleSelection : ISelection
    {
        new Type SelectionType { get; }
        new IEnumerable<object> SelectedObject { get; }
    }
}
