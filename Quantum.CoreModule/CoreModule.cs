using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Services;

namespace Quantum.CoreModule
{
    public class QuantumCoreModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterService<IObjectInitializationService, ObjectInitializationService>();
            container.Resolve<IObjectInitializationService>().RegisterInitializer<ServiceInitializer>();
            container.Resolve<IObjectInitializationService>().RegisterInitializer<SelectionInitializer>();
            container.Resolve<IObjectInitializationService>().RegisterInitializer<SubscriberInitializer>();

            container.RegisterService<IEventAggregator, UnityEventAggregator>();

            container.RegisterService<IWPFEventManagerService, WPFEventManagerService>();
            container.Resolve<IWPFEventManagerService>().HookWpfEvents();

        }
    }
}
