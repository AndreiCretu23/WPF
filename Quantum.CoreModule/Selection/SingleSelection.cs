namespace Quantum.Services
{
    class SingleSelection<T> : SelectionBase
    {
        private T internalValue;
        public T Value
        {
            get {
                return internalValue;
            }
            set {
                OnValueChanging(value, internalValue, BlockNotificationsScope.Value);
                internalValue = value;
                Raise();
            }
        }

        protected virtual void OnValueChanging(T newValue, T oldValue, bool areExternalNotificationsBlocked) { }
    }
}
