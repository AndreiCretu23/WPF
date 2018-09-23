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
        /// <param name="eventAggregator">The event aggregator instance from which to resolve the event.</param>
        /// <param name="eventType">The type of the event that is to be resolved. Supported types are types that extend CompositePresentationEvent or SelectionBase.</param>
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
        /// <param name="eventAggregator">The event aggregator instance from which to resolve the event.</param>
        /// <param name="eventType">The type of the event/selection to which the subscription is made. Supported types are types that extend CompositePresentationEvent or SelectionBase.</param>
        /// <param name="action">The action that is to be invoked when the specified event is fired / the specified selection changes.</param>
        /// <param name="threadOption">The thread on which the event should be handled.</param>
        /// <param name="keepSubscriberReferenceAlive">A flag indicating if a reference to the subscription should be held or not.</param>
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

        /// <summary>
        /// Subscribes the specified action in the given eventAggregator instance to the specified event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <typeparam name="TPayload">The type of the event args.</typeparam>
        /// <param name="eventAggregator">The event aggregator instance from which to resolve the event.</param>
        /// <param name="threadOption">The thread on which the event handler is to be invoked.</param>
        /// <param name="keepSubscriberReferenceAlive">A flag indicating if a reference to the subscription should be held or not.</param>
        /// <param name="action">The handler action to be invoked when the event is fired.</param>
        public static void SubscribeToEvent<TEvent, TPayload>(IEventAggregator eventAggregator, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Action action)
            where TEvent : CompositePresentationEvent<TPayload>
        {
            eventAggregator.GetEvent<TEvent>().Subscribe(payload => action(), threadOption, keepSubscriberReferenceAlive);
        }

        /// <summary>
        /// Subscribes the specified action in the given eventAggregator instance to specified selection changing event.
        /// </summary>
        /// <typeparam name="TSelection">The type of the selection.</typeparam>
        /// <typeparam name="TPayload">The type of the object the selection is wrapping.</typeparam>
        /// <param name="eventAggregator">The event aggregator instance from which to resolve the selection.</param>
        /// <param name="threadOption">The thread on which the event handler is to be invoked.</param>
        /// <param name="keepSubscriberReferenceAlive">A flag indicating if a reference to the subscription should be held or not.</param>
        /// <param name="action">The handler action to be invoked when the selection changes.</param>
        public static void SubscribeToSelection<TSelection, TPayload>(IEventAggregator eventAggregator, ThreadOption threadOption, bool keepSubscriberReferenceAlive, Action action)
            where TSelection : SelectionBase<TPayload>
        {
            eventAggregator.GetEvent<TSelection>().Subscribe(selection => action(), threadOption, keepSubscriberReferenceAlive);
        }

    }
}
