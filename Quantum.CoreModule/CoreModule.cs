using Microsoft.Practices.Composite.Events;
using Quantum.Services;
using Unity;
using Unity.Lifetime;

namespace Quantum.CoreModule
{
    public class QuantumCoreModule : IQuantumModule
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
