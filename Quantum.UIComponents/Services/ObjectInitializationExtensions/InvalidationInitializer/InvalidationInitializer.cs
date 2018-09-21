using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Unity;
using Quantum.Events;
using Quantum.Services;
using Quantum.UIComposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    internal class InvalidationInitializer : IObjectInitializer
    {
        public IUnityContainer Container { get ; set; }
        
        public void Initialize(object obj)
        {
            if (obj == null || !(obj is ObservableObject)) return;

            var notifier = obj as ObservableObject;
            var eventAggregator = Container.Resolve<IEventAggregator>();

            foreach(var prop in notifier.GetType().GetProperties())
            {
                var invalidationAttributes = prop.GetCustomAttributes(false).OfType<InvalidateOnAttribute>();
                foreach(var attribute in invalidationAttributes)
                {
                    eventAggregator.Subscribe(attribute.EventType, () => notifier.RaisePropertyChanged(prop.Name), ThreadOption.UIThread, true);
                }
            }
        }

        public void Teardown(object obj)
        {
        }
    }
}
