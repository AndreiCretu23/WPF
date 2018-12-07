using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Common;

namespace Quantum.Services
{
    public abstract class ServiceBase : IDestructible
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
        /// Tears down all services/event/selections initialized by the IObjectInitializationService
        /// </summary>
        public void TearDown()
        {
            InitializationService.TeardownAll(this);
        }

    }
}
