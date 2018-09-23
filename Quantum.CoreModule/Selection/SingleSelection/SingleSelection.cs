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
                OldValue = new SingleSelectionCache<T>(internalValue);
                internalValue = value;
                Raise();
            }
        }

        /// <summary>
        /// A wrapper that stores the previously selected item. When the value of the selection is set, 
        /// a new wrapper instance is created containing the value of the previously selected item. After that, the 
        /// event is raised and handled by the application. Finally, the wrapper is disposed. <para></para>
        /// WARNING : If a subscriber handles the selection changing on a separate thread, the event raising process will just trigger the handler, 
        ///           and it will not wait for it to finish, meaning the OldValue will get disposed immediately, and will not be accessable inside the handler. 
        /// </summary>
        public SingleSelectionCache<T> OldValue { get; private set; }

        /// <summary>
        /// Exposes the OldValue in the interface as an object.
        /// </summary>
        ISingleSelectionCache ISingleSelection.OldValue { get { return OldValue; } }


        protected override void OnSelectedValueChanged()
        {
            OldValue = null;
        }
    }
    

}
