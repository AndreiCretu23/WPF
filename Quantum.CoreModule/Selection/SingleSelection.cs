namespace Quantum.Services
{
    public abstract class SingleSelection<T> : SelectionBase<T>, ISingleSelection
    {
        public SingleSelection()
            : base()
        {
        }

        public SingleSelection(T defaultValue, bool raiseOnDefaultValueSet = true)
        {
            if(raiseOnDefaultValueSet) {
                Value = defaultValue;
            }
            else {
                internalValue = defaultValue;
            }
        }

        private T internalValue;
        public override T Value
        {
            get { return internalValue; }
            set
            {
                internalValue = value;
                Raise();
            }
        }
    }
}
