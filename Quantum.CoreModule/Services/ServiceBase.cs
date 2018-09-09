using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;

namespace Quantum.Services
{
    public abstract class ServiceBase
    {
        [Service]
        public IUnityContainer Container { get; set; }
        
        [Service]
        public IEventAggregator EventAggregator { get; set; }

        [Service]
        public IObjectInitializationService InitializationService { get; set; }

        public ServiceBase(IObjectInitializationService initSvc)
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
