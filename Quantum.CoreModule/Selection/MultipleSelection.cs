using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quantum.Services
{
    public class MultipleSelection<T> : SelectionBase
    {
        private ObservableCollection<T> internalValue;
        public ObservableCollection<T> Value
        {
            get
            {
                return internalValue;
            }
            set
            {
                OnCollectionReferenceChanging(value, internalValue, BlockNotificationsScope.Value);
                internalValue = value;
                if (internalValue != null) {
                    internalValue.CollectionChanged += (sender, e) => {
                        OnCollectionChanging((IEnumerable<T>)e.NewItems, (IEnumerable<T>)e.OldItems, BlockNotificationsScope.Value);
                        Raise();
                    };
                }
                Raise();
            }
        }

        protected virtual void OnCollectionReferenceChanging(ObservableCollection<T> newValue, ObservableCollection<T> oldValue, bool areExternalNotificationsBlocked) { }

        protected virtual void OnCollectionChanging(IEnumerable<T> newItems, IEnumerable<T> oldItems, bool areExternalNotificationsBlocked) { }
        
    }
}
