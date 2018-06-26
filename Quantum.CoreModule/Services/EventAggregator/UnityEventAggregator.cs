using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;

namespace Quantum.Services
{
    public class UnityEventAggregator : IEventAggregator
    {
        [Service]
        public IUnityContainer Container { get; set; }

        public UnityEventAggregator(IUnityContainer container)
        {
            Container = container;
        }

        public TEventType GetEvent<TEventType>() where TEventType : EventBase
        {
            return Container.Resolve<TEventType>();
        }
    }
}
