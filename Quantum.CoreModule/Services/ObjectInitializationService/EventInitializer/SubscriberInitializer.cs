using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Microsoft.Practices.Unity;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Quantum.Exceptions;

namespace Quantum.Services
{
    internal class SubscriberInitializer : IObjectInitializer
    {
        public IUnityContainer Container { get; set; }

        private Dictionary<object, List<Subscription>> InitializationCache { get; set; } = new Dictionary<object, List<Subscription>>();

        public void Initialize(object obj)
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            
            var handlerMethods = obj.GetType().GetMethods().Where(method => method.HasAttribute<HandlesAttribute>());

            if(handlerMethods.Any())
            {
                InitializationCache.Add(obj, new List<Subscription>());
            }

            foreach(var handlerMethod in handlerMethods)
            {
                var handlerLibrary = handlerMethod.GetCustomAttributes().OfType<HandlesAttribute>();
                if(handlerLibrary.Count() > 1 && handlerMethod.GetParameters().Any())
                {
                    throw new InvalidMethodParametersException($"{GetMemberBasicInfo(obj, handlerMethod)} : \n" +
                                        $"A method that handles multiple events cannot have parameters!");
                }

                foreach(var handleInfo in handlerLibrary)
                {
                    if(handleInfo.EventType.IsSubclassOfRawGeneric(typeof(SelectionBase<>))) {
                        SubscribeSelection(obj, eventAggregator, handlerMethod, handleInfo);
                    }

                    else if(handleInfo.EventType.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>))) {
                        SubscribeEvent(obj, eventAggregator, handlerMethod, handleInfo);
                    }
                    
                    else {
                        throw new UnexpectedTypeException(typeof(EventBase), handleInfo.EventType, "Error : A method can only handle Selections and CompositePresentationEvents.");
                    }
                }
            }
        }

        private void SubscribeEvent(object obj, IEventAggregator eventAggregator, MethodInfo handler, HandlesAttribute handleInfo)
        {
            var getEvtMethodName = ReflectionUtils.GetMethodName((IEventAggregator evtAggregator) => (Func<EventBase>)evtAggregator.GetEvent<EventBase>);
            var evt = typeof(IEventAggregator).GetMethod(getEvtMethodName).MakeGenericMethod(new Type[] { handleInfo.EventType }).Invoke(eventAggregator, new object[] { });
            var eventType = handleInfo.EventType;
            var payloadType = handleInfo.EventType.GetBaseTypeGenericArgument(typeof(CompositePresentationEvent<>));
            
            if(!handler.GetParameters().Any()) {
                var subscriptionProxy = new SubscriptionProxy(eventAggregator);
                var subscriptionMethodName = ReflectionUtils.GetMethodName((SubscriptionProxy proxy) => (Func<object, MethodInfo, ThreadOption, bool, SubscriptionToken>)proxy.SubscribeEventHandler<CompositePresentationEvent<object>, object>);
                var subscriptionMethod = typeof(SubscriptionProxy).GetMethod(subscriptionMethodName).MakeGenericMethod(new Type[] { eventType, payloadType });
                var token = (SubscriptionToken)subscriptionMethod.Invoke(subscriptionProxy, new object[] { obj, handler, handleInfo.ThreadOption, handleInfo.KeepSubscriberReferenceAlive });
                InitializationCache[obj].Add(new Subscription()
                {
                    Object = obj, 
                    Event = (EventBase)evt, 
                    Token = token,
                });
            }
            else if(handler.GetParameters().Count() == 1) {
                var subscriptionProxy = new SubscriptionProxy(eventAggregator);
                var subscriptionMethodName = ReflectionUtils.GetMethodName((SubscriptionProxy proxy) => (Func<object, MethodInfo, ThreadOption, bool, SubscriptionToken>)proxy.SubscribeEventHandlerArgs<CompositePresentationEvent<object>, object>);
                var subscriptionMethod = typeof(SubscriptionProxy).GetMethod(subscriptionMethodName).MakeGenericMethod(new Type[] { eventType, payloadType });
                var token = (SubscriptionToken)subscriptionMethod.Invoke(subscriptionProxy, new object[] { obj, handler, handleInfo.ThreadOption, handleInfo.KeepSubscriberReferenceAlive });
                InitializationCache[obj].Add(new Subscription()
                {
                    Object = obj,
                    Event = (EventBase)evt,
                    Token = token,
                });
            }
            else {
                throw new InvalidMethodParametersException("An event handler method cannot have more than 1 parameter, which must be of the args type of the handled event.");
            }
        }
        
        private void SubscribeSelection(object obj, IEventAggregator eventAggregator, MethodInfo handler, HandlesAttribute handleInfo)
        {
            var getEvtMethodName = ReflectionUtils.GetMethodName((IEventAggregator evtAggregator) => (Func<EventBase>)evtAggregator.GetEvent<EventBase>);
            var selection = typeof(IEventAggregator).GetMethod(getEvtMethodName).MakeGenericMethod(new Type[] { handleInfo.EventType }).Invoke(eventAggregator, new object[] { });
            var selectionType = handleInfo.EventType;
            var payloadType = handleInfo.EventType.GetBaseTypeGenericArgument(typeof(SelectionBase<>));

            if (!handler.GetParameters().Any()) {
                var subscriptionProxy = new SubscriptionProxy(eventAggregator);
                var subscriptionMethodName = ReflectionUtils.GetMethodName((SubscriptionProxy proxy) => (Func<object, MethodInfo, ThreadOption, bool, SubscriptionToken>)proxy.SubscribeSelectionChangedHandler<SelectionBase<object>, object>);
                var subscriptionMethod = typeof(SubscriptionProxy).GetMethod(subscriptionMethodName).MakeGenericMethod(new Type[] { selectionType, payloadType });
                var token = (SubscriptionToken)subscriptionMethod.Invoke(subscriptionProxy, new object[] { obj, handler, handleInfo.ThreadOption, handleInfo.KeepSubscriberReferenceAlive});
                InitializationCache[obj].Add(new Subscription()
                {
                    Object = obj,
                    Event = (EventBase)selection,
                    Token = token,
                });
            }
            else if(handler.GetParameters().Count() == 1) {
                var subscriptionProxy = new SubscriptionProxy(eventAggregator);
                var subscriptionMethodName = ReflectionUtils.GetMethodName((SubscriptionProxy proxy) => (Func<object, MethodInfo, ThreadOption, bool, SubscriptionToken>)proxy.SubscribeSelectionChangedHandlerArgs<SelectionBase<object>, object>);
                var subscriptionMethod = typeof(SubscriptionProxy).GetMethod(subscriptionMethodName).MakeGenericMethod(new Type[] { selectionType, payloadType });
                var token = (SubscriptionToken)subscriptionMethod.Invoke(subscriptionProxy, new object[] { obj, handler, handleInfo.ThreadOption, handleInfo.KeepSubscriberReferenceAlive });
                InitializationCache[obj].Add(new Subscription()
                {
                    Object = obj,
                    Event = (EventBase)selection,
                    Token = token,
                });
            }
            else {
                throw new InvalidMethodParametersException("An event handler method cannot have more than 1 parameter, which must be of the selection type.");
            }
        }
        
        private string GetMemberBasicInfo(object obj, MethodInfo method)
        {
            return $"Handle Event exception in {obj.GetType().Name}, Method {method.Name}";
        }

        public void Teardown(object obj)
        {
            if(InitializationCache.ContainsKey(obj))
            {
                var subscriptions = InitializationCache[obj];
                foreach(var sub in subscriptions)
                {
                    sub.Event.Unsubscribe(sub.Token);
                }
                subscriptions.Clear();
                InitializationCache.Remove(obj);
            }
        }
    }

    internal class SubscriptionProxy
    {
        public IEventAggregator EventAggregator { get; private set; }

        public SubscriptionProxy(IEventAggregator eventAggregator) {
            EventAggregator = eventAggregator;
        }

        public SubscriptionToken SubscribeEventHandler<TEvent, TPayload>(object obj, MethodInfo handler, ThreadOption threadOption, bool keepSubscriberReferencesAlive)
            where TEvent : CompositePresentationEvent<TPayload>
        {
            return EventAggregator.GetEvent<TEvent>().Subscribe(args => handler.Invoke(obj, new object[] { }), threadOption, keepSubscriberReferencesAlive);
        }

        public SubscriptionToken SubscribeEventHandlerArgs<TEvent, TPayload>(object obj, MethodInfo handler, ThreadOption threadOption, bool keepSubscriberReferencesAlive)
            where TEvent : CompositePresentationEvent<TPayload>
        {
            return EventAggregator.GetEvent<TEvent>().Subscribe(args => handler.Invoke(obj, new object[] { args }), threadOption, keepSubscriberReferencesAlive);
        }

        public SubscriptionToken SubscribeSelectionChangedHandler<TSelection, TPayload>(object obj, MethodInfo handler, ThreadOption threadOption, bool keepSubscriberReferencesAlive)
            where TSelection : SelectionBase<TPayload>
        {
            return EventAggregator.GetEvent<TSelection>().Subscribe(selection => handler.Invoke(obj, new object[] { }), threadOption, keepSubscriberReferencesAlive);
        }

        public SubscriptionToken SubscribeSelectionChangedHandlerArgs<TSelection, TPayload>(object obj, MethodInfo handler, ThreadOption threadOption, bool keepSubscriberReferencesAlive)
            where TSelection : SelectionBase<TPayload>
        {
            return EventAggregator.GetEvent<TSelection>().Subscribe(selection => handler.Invoke(obj, new object[] { (TSelection)selection }), threadOption, keepSubscriberReferencesAlive);
        }
    }

    internal class Subscription
    {
        public object Object { get; set; }
        public EventBase Event { get; set; }
        public SubscriptionToken Token { get; set; }
    }

}
