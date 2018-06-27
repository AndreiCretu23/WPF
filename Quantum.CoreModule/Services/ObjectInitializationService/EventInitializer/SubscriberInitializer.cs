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

        public void Initialize(object obj)
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            
            var handlerMethods = obj.GetType().GetMethods().Where(method => method.HasAttribute<HandlesAttribute>());

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
            var evt = typeof(IEventAggregator).GetMethod("GetEvent").MakeGenericMethod(new Type[] { handleInfo.EventType }).Invoke(eventAggregator, new object[] { });
            var eventType = handleInfo.EventType;
            var payloadType = handleInfo.EventType.GetBaseTypeGenericArgument(typeof(CompositePresentationEvent<>));
            
            if(!handler.GetParameters().Any()) {
                var subscriptionProxy = new SubscriptionProxy(eventAggregator);
                var subscriptionMethod = typeof(SubscriptionProxy).GetMethod("SubscribeEventHandler").MakeGenericMethod(new Type[] { eventType, payloadType });
                subscriptionMethod.Invoke(subscriptionProxy, new object[] { obj, handler, handleInfo.ThreadOption, handleInfo.KeepSubscriberReferenceAlive });
            }
            else if(handler.GetParameters().Count() == 1) {
                var subscriptionProxy = new SubscriptionProxy(eventAggregator);
                var subscriptionMethod = typeof(SubscriptionProxy).GetMethod("SubscribeEventHandlerArgs").MakeGenericMethod(new Type[] { eventType, payloadType });
                subscriptionMethod.Invoke(subscriptionProxy, new object[] { obj, handler, handleInfo.ThreadOption, handleInfo.KeepSubscriberReferenceAlive });
            }
            else {
                throw new InvalidMethodParametersException("An event handler method cannot have more than 1 parameter, which must be of the args type of the handled event.");
            }
        }
        
        private void SubscribeSelection(object obj, IEventAggregator eventAggregator, MethodInfo handler, HandlesAttribute handleInfo)
        {
            var selection = typeof(IEventAggregator).GetMethod("GetEvent").MakeGenericMethod(new Type[] { handleInfo.EventType }).Invoke(eventAggregator, new object[] { });
            var selectionType = handleInfo.EventType;
            var payloadType = handleInfo.EventType.GetBaseTypeGenericArgument(typeof(SelectionBase<>));

            if (!handler.GetParameters().Any()) {
                var subscriptionProxy = new SubscriptionProxy(eventAggregator);
                var subscriptionMethod = typeof(SubscriptionProxy).GetMethod("SubscribeSelectionChangedHandler").MakeGenericMethod(new Type[] { selectionType, payloadType });
                subscriptionMethod.Invoke(subscriptionProxy, new object[] { obj, handler, handleInfo.ThreadOption, handleInfo.KeepSubscriberReferenceAlive});
            }
            else if(handler.GetParameters().Count() == 1) {
                var subscriptionProxy = new SubscriptionProxy(eventAggregator);
                var subscriptionMethod = typeof(SubscriptionProxy).GetMethod("SubscribeSelectionChangedHandlerArgs").MakeGenericMethod(new Type[] { selectionType, payloadType });
                subscriptionMethod.Invoke(subscriptionProxy, new object[] { obj, handler, handleInfo.ThreadOption, handleInfo.KeepSubscriberReferenceAlive });
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
            // TODO : Implement Unsubscription Mechanism
        }
    }

    internal class SubscriptionProxy
    {
        public IEventAggregator EventAggregator { get; private set; }

        public SubscriptionProxy(IEventAggregator eventAggregator) {
            EventAggregator = eventAggregator;
        }

        public void SubscribeEventHandler<TEvent, TPayload>(object obj, MethodInfo handler, ThreadOption threadOption, bool keepSubscriberReferencesAlive)
            where TEvent : CompositePresentationEvent<TPayload>
        {
            EventAggregator.GetEvent<TEvent>().Subscribe(args => handler.Invoke(obj, new object[] { }), threadOption, keepSubscriberReferencesAlive);
        }

        public void SubscribeEventHandlerArgs<TEvent, TPayload>(object obj, MethodInfo handler, ThreadOption threadOption, bool keepSubscriberReferencesAlive)
            where TEvent : CompositePresentationEvent<TPayload>
        {
            EventAggregator.GetEvent<TEvent>().Subscribe(args => handler.Invoke(obj, new object[] { args }), threadOption, keepSubscriberReferencesAlive);
        }

        public void SubscribeSelectionChangedHandler<TSelection, TPayload>(object obj, MethodInfo handler, ThreadOption threadOption, bool keepSubscriberReferencesAlive)
            where TSelection : SelectionBase<TPayload>
        {
            EventAggregator.GetEvent<TSelection>().Subscribe(selection => handler.Invoke(obj, new object[] { }), threadOption, keepSubscriberReferencesAlive);
        }

        public void SubscribeSelectionChangedHandlerArgs<TSelection, TPayload>(object obj, MethodInfo handler, ThreadOption threadOption, bool keepSubscriberReferencesAlive)
            where TSelection : SelectionBase<TPayload>
        {
            EventAggregator.GetEvent<TSelection>().Subscribe(selection => handler.Invoke(obj, new object[] { (TSelection)selection }), threadOption, keepSubscriberReferencesAlive);
        }
    }
}
