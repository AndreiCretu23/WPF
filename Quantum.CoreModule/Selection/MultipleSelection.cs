using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quantum.Services
{
    public abstract class MultipleSelection<T> : SelectionBase<ObservableCollection<T>>, IMultipleSelection
    {
        public MultipleSelection()
        {
            internalValue = new ObservableCollection<T>();
            internalValue.CollectionChanged += (sender, e) => Raise();
        }

        public MultipleSelection(ObservableCollection<T> defaultValue, bool raiseOnDefaultValueSet = true)
        {
            if(raiseOnDefaultValueSet) {
                Value = defaultValue;
            }
            else {
                internalValue = defaultValue;
                internalValue.CollectionChanged += (sender, e) => Raise();
            }
        }

        private ObservableCollection<T> internalValue;
        public override ObservableCollection<T> Value
        {
            get { return internalValue; }
            set
            {
                internalValue = value;
                internalValue.IfNotNull(o => o.CollectionChanged += (sender, e) => Raise());
                Raise();
            }
        }


        IEnumerable<object> IMultipleSelection.SelectedObject => Value.Cast<object>();
        Type IMultipleSelection.SelectionType => typeof(T);
    }
}
