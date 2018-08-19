using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Utils;
using System;

namespace Quantum.Services
{
    public static class EventAggregatorHelpers
    {
        /// <summary>
        /// Returns the instance of the specified event type (from the container), performing a registration check first. 
        /// It is the equivalent of EventAggregator.GetEvent'1, but this can be called on runtime.
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static object GetEvent(this IEventAggregator eventAggregator, Type eventType)
        {
            eventAggregator.AssertNotNull(nameof(eventAggregator));
            eventType.AssertParameterNotNull(nameof(eventType));

            if(!(eventType.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>)) || 
                 eventType.IsSubclassOfRawGeneric(typeof(SelectionBase<>))))
            {
                throw new NotSupportedException($"Error : {eventType.Name} is not a supported eventType. Supported types are either subtypes of CompositePresentationEvent<T> (events) or " +
                                                $"subtypes of SelectionBase<T>(selections).");
            }

            var eventGetter = typeof(IEventAggregator).GetMethod("GetEvent").MakeGenericMethod(eventType);
            return eventGetter.Invoke(eventAggregator, new object[] { });
        }

        /// <summary>
        /// Subscribes the given action to the given event/selection type. Since the eventType is determined at runtime, the event/selection arguments are not available : 
        /// you can just subscribe a parameterless action.
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        /// <param name="threadOption"></param>
        /// <param name="keepSubscriberReferenceAlive"></param>
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
