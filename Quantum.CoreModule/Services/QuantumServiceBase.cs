using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;

namespace Quantum.Services
{
    public abstract class QuantumServiceBase
    {
        [Service]
        public IUnityContainer Container { get; set; }
        
        [Service]
        public IEventAggregator EventAggregator { get; set; }

        public QuantumServiceBase(IObjectInitializationService initSvc)
        {
            initSvc.Initialize(this);
        }

    }
}
