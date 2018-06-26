namespace Quantum.Services
{
    public abstract class SingleSelection<T> : SelectionBase<T>
    {
        public SingleSelection(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public SingleSelection(IObjectInitializationService initSvc, T defaultValue, bool raiseOnDefaultValueSet = false)
            : base(initSvc, defaultValue, raiseOnDefaultValueSet)
        {
        }
    }
}
