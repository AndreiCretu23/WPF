using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quantum.Services
{
    /// <summary>
    /// The base class for all multiple selections. A multiple selection is an event-wrapper class over a 
    /// collection of items. Whenever the collection changes, the event raises itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MultipleSelection<T> : SelectionBase<IEnumerable<T>>, IMultipleSelection
    {
        /// <summary>
        /// Creates a new instance of the multiple selection class and instantiates the internal collection without raising the event.
        /// </summary>
        public MultipleSelection()
        {
        }

        /// <summary>
        /// Creates a new instance of the multiple selection class and sets the default collection to the given values.
        /// </summary>
        /// <param name="defaultValue">The default value of the collection.</param>
        /// <param name="raiseOnDefaultValueSet">A flag indicating if the event should raise itself when the default value is set.</param>
        public MultipleSelection(IEnumerable<T> defaultValue, bool raiseOnDefaultValueSet = false)
        {
            if (raiseOnDefaultValueSet)
            {
                using (BeginBlockingNotifications())
                {
                    foreach(var value in defaultValue)
                    {
                        Add(value);
                    }
                }
            }
            else
            {
                foreach(var value in defaultValue)
                {
                    internalValues.Add(value);
                }
            }
        }

        private List<T> internalValues { get; } = new List<T>();
        /// <summary>
        /// Returns the collection of values currently inside the selection. Setting this property will adapt 
        /// the selection to the given values, dipose the old ones and raise the event.
        /// </summary>
        public override IEnumerable<T> Value
        {
            get { return internalValues; }
            set 
            {
                using (BeginBlockingNotifications())
                {
                    foreach(var currentValue in internalValues.ToList())
                    {
                        Remove(currentValue);
                    }
                    foreach(var newValue in value)
                    {
                        Add(newValue);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the given value to the selection collection and notifies the listeners that the selection has changed.
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            if(ChangedItems == null)
            {
                ChangedItems = new MultipleSelectionCache<T>();
            }
            internalValues.Add(value);
            ChangedItems.addedValues.Add(value);
            Raise();
        }

        /// <summary>
        /// Attempts to remove the specified value from the selection, and if successful, it nofifies the listeners that the selection has changed.
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value)
        {
            if(internalValues.Remove(value))
            {
                if (ChangedItems == null)
                {
                    ChangedItems = new MultipleSelectionCache<T>();
                }
                ChangedItems.removedValues.Add(value);
                Raise();
            }
        }

        /// <summary>
        /// A wrapper that stores the the added and removed items of the current selection changing scope. When items are added or removed from the selection, 
        /// a new wrapper instance is created in which the newly added and removed items are stored. If the add/remove operations are done within a 
        /// notification blocking scope, then the changed will stack up inside the cache instance. Else the added/removed item will be added in the corresponding
        /// collection of the cache. Either after the added/removed item is added in the cache(not using a blocking notification scope), or the selection 
        /// blocking notification scope ends, the event will be raised and handled by the application. Finally, the wrapper is disposed.<para></para>
        /// WARNING : If a subscriber handles the selection changing on a separate thread, the event raising process will just trigger the handler, 
        ///           and it will not wait for it to finish, meaning the ChangedItem cache will get disposed immediately, and will not be accessible inside the handler. 
        /// </summary>
        public MultipleSelectionCache<T> ChangedItems { get; private set; }

        protected override void OnSelectedValueChanged()
        {
            ChangedItems = null;
        }

        #region IMultipleSelection Implementation

        Type IMultipleSelection.SelectionType => typeof(T);

        IEnumerable<object> IMultipleSelection.SelectedObject => Value.Cast<object>();

        IMultipleSelectionCache IMultipleSelection.ChangedItems => ChangedItems;

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

        #endregion IMultipleSelection Implementation
    }
}
