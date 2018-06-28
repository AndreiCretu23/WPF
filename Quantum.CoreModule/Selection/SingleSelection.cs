namespace Quantum.Services
{
    public abstract class SingleSelection<T> : SelectionBase<T>
    {
        public SingleSelection()
            : base()
        {
        }
        
        public SingleSelection(IObjectInitializationService initSvc, T defaultValue, bool raiseOnDefaultValueSet = false)
            : base(defaultValue, raiseOnDefaultValueSet)
        {
        }
    }
}
