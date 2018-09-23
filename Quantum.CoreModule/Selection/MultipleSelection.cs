using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quantum.Services
{
    /// <summary>
    /// The base class for all multiple selections. A multiple selection is an event-wrapper class over a 
    /// collection of items. Whenever the collection itself or an item inside the collection changes, 
    /// the event raises itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MultipleSelection<T> : SelectionBase<ObservableCollection<T>>, IMultipleSelection
    {
        /// <summary>
        /// Creates a new instance of the multiple selection class and instantiates the internal collection without raising the event.
        /// </summary>
        public MultipleSelection()
        {
            internalValue = new ObservableCollection<T>();
            internalValue.CollectionChanged += (sender, e) => Raise();
        }

        /// <summary>
        /// Creates a new instance of the multiple selection class and sets the default collection to the given instance.
        /// </summary>
        /// <param name="defaultValue">The default value of the collection.</param>
        /// <param name="raiseOnDefaultValueSet">A flag indicating if the event should raise itself when the default value is set.</param>
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
        /// <summary>
        /// The collection of values currently inside the selection.
        /// </summary>
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

        /// <summary>
        /// Adds the given value to the selection collection and raises the event.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value) {
            Value.Add(value);
        }

        /// <summary>
        /// Removes the selected value from the selection and raises the event.
        /// </summary>
        /// <param name="value"></param>
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
