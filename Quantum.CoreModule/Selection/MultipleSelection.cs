using System.Collections.ObjectModel;

namespace Quantum.Services
{
    public abstract class MultipleSelection<T> : SelectionBase<ObservableCollection<T>>
    {
        public MultipleSelection()
            : base(new ObservableCollection<T>(), false)
        {
        }
        
        public MultipleSelection(IObjectInitializationService initSvc, ObservableCollection<T> defaultValue, bool raiseOnDefaultValueSet = false)
            : base(defaultValue, raiseOnDefaultValueSet)
        {
        }

        protected override void OnSelectedValueChanged()
        {
            Value.CollectionChanged += (sender, e) => Raise();
        }
    }
}
