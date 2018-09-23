using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Services
{
    public class MultipleSelectionCache<T> : IMultipleSelectionCache
    {
        internal List<T> removedValues { get; set; } = new List<T>();
        internal List<T> addedValues { get; set; } = new List<T>();

        public IEnumerable<T> RemovedValues { get { return removedValues; } }
        public IEnumerable<T> AddedValues { get { return addedValues; } }

        IEnumerable<object> IMultipleSelectionCache.RemovedValues => RemovedValues.Cast<object>();
        IEnumerable<object> IMultipleSelectionCache.AddedValues => AddedValues.Cast<object>();
    }
}
