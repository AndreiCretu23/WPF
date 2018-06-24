using Microsoft.Practices.Composite.Events;
using Unity;
using Unity.Lifetime;

namespace Quantum.Core.Services
{
    internal class BaseModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            
            container.RegisterType<IObjectInitializationService, ObjectInitializationService>(new ContainerControlledLifetimeManager());
            container.Resolve<IObjectInitializationService>().RegisterInitializer<ServiceInitializer>();
            container.Resolve<IObjectInitializationService>().RegisterInitializer<SubscriberInitializer>();
        }
    }
}
