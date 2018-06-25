using Microsoft.Practices.Composite.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Quantum.Services
{
    public class UnityEventAggregator : QuantumServiceBase, IEventAggregator
    {
        public UnityEventAggregator(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public TEventType GetEvent<TEventType>() where TEventType : EventBase
        {
            return Container.Resolve<TEventType>();
        }
    }
}
