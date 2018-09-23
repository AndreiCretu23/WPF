namespace Quantum.Services
{
    /// <summary>
    /// The base class for all single selections. A single selection is an event-wrapper class 
    /// over an object. It holds a reference to an object an whenever that object changes (the value property is set), 
    /// the event raises itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleSelection<T> : SelectionBase<T>, ISingleSelection
    {
        /// <summary>
        /// Creates a new instance of the SingleSelection class.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="raiseOnDefaultValueSet"></param>
        public SingleSelection()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the SingleSelection class.
        /// </summary>
        /// <param name="defaultValue">The default value for this selection.</param>
        /// <param name="raiseOnDefaultValueSet">A flag indicating if the event should raise itself when the default value is set.</param>
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
        /// <summary>
        /// A reference to the value currently stored inside the instance of the selection.
        /// </summary>
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
