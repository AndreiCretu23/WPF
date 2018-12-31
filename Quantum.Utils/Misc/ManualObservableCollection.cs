using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Utils
{
    /// <summary>
    /// Represents an observable collection in which the collection changed event is triggered manually.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ManualObservableCollection<T> : ObservableCollection<T>
    {

        public void AddRange(IEnumerable<T> collection)
        {
            collection.AssertParameterNotNull(nameof(collection));
            foreach(var element in collection) {
                Add(element);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Do nothing
        }

        public void RaiseCollectionChanged()
        {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

}
