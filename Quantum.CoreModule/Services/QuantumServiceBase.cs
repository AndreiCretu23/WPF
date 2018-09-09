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

        [Service]
        public IObjectInitializationService InitializationService { get; set; }

        public QuantumServiceBase(IObjectInitializationService initSvc)
        {
            initSvc.Initialize(this);
        }

        /// <summary>
        /// Tears down all services/events/selections initialized by the IObjectInitializationService
        /// </summary>
        protected void TearDown()
        {
            InitializationService.TeardownAll(this);
        }

    }
}
