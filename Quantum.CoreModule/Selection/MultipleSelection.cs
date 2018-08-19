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


        public void Add(T value) {
            Value.Add(value);
        }

        public void Remove(T value) {
            Value.Remove(value);
        }


        IEnumerable<object> IMultipleSelection.SelectedObject => Value.Cast<object>();
        Type IMultipleSelection.SelectionType => typeof(T);

        void IMultipleSelection.Add(object value)
        {
            Add(value.SafeCast<T>($"Error : The specified value does not match the type of the selection : \n " +
                                  $"Selection type is : {typeof(T).Name} \n " +
                                  $"Actual type is : {value.GetType().Name}"));
        }

        void IMultipleSelection.Remove(object value)
        {
            Remove(value.SafeCast<T>($"Error : The specified value does not match the type of the selection : \n " +
                                     $"Selection type is : {typeof(T).Name} \n " +
                                     $"Actual type is : {value.GetType().Name}"));
        }
    }
}
