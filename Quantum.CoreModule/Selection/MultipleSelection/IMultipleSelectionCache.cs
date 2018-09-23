using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Services
{
    public interface IMultipleSelectionCache
    {
        IEnumerable<object> RemovedValues { get; }
        IEnumerable<object> AddedValues { get; }
    }
}
