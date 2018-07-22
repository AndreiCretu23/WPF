using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Services
{
    public static class EventAggregatorHelpers
    {

        public static void Subscribe(this IEventAggregator eventAggregator, Type eventType, Action action, ThreadOption threadOption = ThreadOption.PublisherThread, bool keepSubscriberReferenceAlive = true)
        {
            eventAggregator.AssertNotNull(nameof(eventAggregator));
            eventType.AssertParameterNotNull(nameof(eventType));
            action.AssertParameterNotNull(nameof(action));

            if(eventType.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>)))
            {
                var payloadType = eventType.GetBaseTypeGenericArgument(typeof(CompositePresentationEvent<>));
                var subscriptionMethod = typeof(EventAggregatorHelpers).GetMethod("SubscribeToEvent").MakeGenericMethod(new Type[] { eventType, payloadType });
                subscriptionMethod.Invoke(null, new object[] { eventAggregator, threadOption, keepSubscriberReferenceAlive, action });
            }

            else if(eventType.IsSubclassOfRawGeneric(typeof(SelectionBase<>)))
            {
                var payloadType = eventType.GetBaseTypeGenericArgument(typeof(SelectionBase<>));
                var subscriptionMethod = typeof(EventAggregatorHelpers).GetMethod("SubscribeToSelection").MakeGenericMethod(new Type[] { eventType, payloadType });
                subscriptionMethod.Invoke(null, new object[] { eventAggregator, threadOption, keepSubscriberReferenceAlive, action });
            }

            else
            {
                throw new NotSupportedException($"{eventType.Name} is not a supported event type.");
            }
        }
        
        public static void SubscribeToEvent<TEvent, TPayload>(IEventAggregator eventAggregator, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Action action)
            where TEvent : CompositePresentationEvent<TPayload>
        {
            eventAggregator.GetEvent<TEvent>().Subscribe(payload => action(), threadOption, keepSubscriberReferenceAlive);
        }

        public static void SubscribeToSelection<TSelection, TPayload>(IEventAggregator eventAggregator, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Action action)
            where TSelection : SelectionBase<TPayload>
        {
            eventAggregator.GetEvent<TSelection>().Subscribe(selection => action(), threadOption, keepSubscriberReferenceAlive);
        }

    }
}
