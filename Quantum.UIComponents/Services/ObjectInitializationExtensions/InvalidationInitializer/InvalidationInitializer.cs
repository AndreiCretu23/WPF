using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Unity;
using Quantum.Exceptions;
using Quantum.Services;
using Quantum.UIComposition;
using Quantum.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.UIComponents
{
    internal class InvalidationInitializer : IObjectInitializer
    {
        public IUnityContainer Container { get ; set; }

        private Dictionary<ObservableObject, List<Subscription>> InitializedObjects { get; set; } = new Dictionary<ObservableObject, List<Subscription>>();

        public void Initialize(object obj)
        {
            if (obj == null) return;

            var targetProperties = obj.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(InvalidateOnAttribute), false));
            if(!(obj is ObservableObject))
            {
                if(targetProperties.Any())
                {
                    var propertyNames = string.Join(",", targetProperties.Select(prop => prop.Name));
                    throw new UnexpectedTypeException(typeof(ObservableObject), obj.GetType(), 
                                                     $"Error : {obj.GetType().Name}, Properties {propertyNames} : \n" +
                                                     $"InvalidateOnAttribute is only allowed on properties contained in types that extent {nameof(ObservableObject)}");
                }
                return;
            }

            var notifier = obj as ObservableObject;
            var eventAggregator = Container.Resolve<IEventAggregator>();

            if(!InitializedObjects.ContainsKey(notifier))
            {
                InitializedObjects.Add(notifier, new List<Subscription>());
            }

            foreach(var prop in targetProperties)
            {
                var invalidationAttributes = prop.GetCustomAttributes(false).OfType<InvalidateOnAttribute>();
                foreach(var attribute in invalidationAttributes)
                {
                    if(attribute.EventType == null || !(attribute.EventType.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>)) || 
                                                        attribute.EventType.IsSubclassOfRawGeneric(typeof(SelectionBase<>))))
                    {
                        throw new UnexpectedTypeException(typeof(EventBase), attribute.EventType,
                                                         $"Error : {notifier.GetType().Name}.{prop.Name} : \n " +
                                                         $"InvalidateOnAttribute : The given event type is invalid. Supported event types are types which are not null and extent CompositePresentationEvent<T> or SelectionBase<T>.");
                    }

                    var token = eventAggregator.Subscribe(attribute.EventType, () => notifier.RaisePropertyChanged(prop.Name), ThreadOption.UIThread, true);
                    InitializedObjects[notifier].Add(new Subscription()
                    {
                        Object = notifier, 
                        Event = eventAggregator.GetEvent(attribute.EventType), 
                        Token = token
                    });
                }
            }
        }

        public void Teardown(object obj)
        {
            if(obj != null && obj is ObservableObject notifier && InitializedObjects.ContainsKey(notifier))
            {
                foreach(var sub in InitializedObjects[notifier])
                {
                    sub.Event.Unsubscribe(sub.Token);
                }
                InitializedObjects[notifier].Clear();
                InitializedObjects.Remove(notifier);
            }
            
        }
    }


}
