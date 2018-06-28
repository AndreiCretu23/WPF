using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;

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


        private List<Type> RegisteredEventTypes { get; set; } = new List<Type>();
        public TEventType GetEvent<TEventType>() where TEventType : EventBase
        {
            if(!RegisteredEventTypes.Contains(typeof(TEventType))) {
                Container.RegisterService<TEventType>();
                RegisteredEventTypes.Add(typeof(TEventType));
            }
            return Container.Resolve<TEventType>();
        }
    }
}
