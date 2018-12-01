using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Common;

namespace Quantum.Services
{
    public abstract class ServiceBase : IInitializableObject
    {
        [Service]
        public IUnityContainer Container { get; set; }
        
        [Service]
        public IEventAggregator EventAggregator { get; set; }

        [Service]
        public IObjectInitializationService InitializationService { get; set; }

        /// <summary>
        /// Returns a value indicating if this object has been initialized or not by the IObjectInitializeService.
        /// </summary>
        public bool IsInitialized { get; private set; }

        public ServiceBase(IObjectInitializationService initSvc)
        {
            Initialize(initSvc);
        }

        /// <summary>
        /// Initializes the object using the given IObjectInitializeService instance.
        /// </summary>
        /// <param name="initSvc"></param>
        public void Initialize(IObjectInitializationService initSvc)
        {
            if(IsInitialized) {
                TearDown();
            }

            initSvc.Initialize(this);
            IsInitialized = true;
        }


        /// <summary>
        /// Tears down all services/events/selections initialized by the IObjectInitializationService
        /// </summary>
        public void TearDown()
        {
            InitializationService.TeardownAll(this);
            IsInitialized = false;
        }

    }
}
