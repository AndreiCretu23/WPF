using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Unity;

namespace Quantum.Core.Services
{
    internal class SubscriberInitializer : IObjectInitializer
    {
        public IUnityContainer Container { get; set; }

        public void Initialize(object obj)
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            var eventGetter = typeof(IEventAggregator).GetMethod("GetEvent");

            var handlerMethods = obj.GetType().GetMethods().Where(method => method.GetCustomAttributes().OfType<HandlesAttribute>().Any());

            foreach (var handlerMethod in handlerMethods)
            {
                var handlerLibrary = handlerMethod.GetCustomAttributes().OfType<HandlesAttribute>();
                if (handlerLibrary.Count() > 1 && handlerMethod.GetParameters().Count() != 0)
                {
                    throw new Exception($"{GetMemberBasicInfo(obj, handlerMethod)} : \n" +
                                        $"A method that handles multiple events cannot have parameters!");
                }

                foreach (var handlerInfo in handlerLibrary)
                {
                    var eventType = handlerInfo.EventType;
                    var e = eventGetter.MakeGenericMethod(eventType).Invoke(eventAggregator, new object[] { });

                    var baseEventType = eventType;

                    while (baseEventType != null)
                    {
                        if (baseEventType.GetGenericArguments().Count() != 1)
                        {
                            baseEventType = baseEventType.BaseType;
                            continue;
                        }
                        else if (baseEventType.GetGenericTypeDefinition() == typeof(CompositePresentationEvent<>))
                        {
                            break;
                        }
                    }

                    if (baseEventType == null)
                    {
                        throw new Exception($"{GetMemberBasicInfo(obj, handlerMethod)} The event type must extend CompositePresentationEvent<TArgs>");
                    }

                    var argsType = baseEventType.GenericTypeArguments.Single();

                    var handlerParameters = handlerMethod.GetParameters();


                    if (handlerMethod.ReturnType != typeof(void))
                    {
                        throw new Exception($"{GetMemberBasicInfo(obj, handlerMethod)} : \n" +
                                            $"The return type must be void.");
                    }
                    else if (handlerParameters.Count() == 0)
                    {
                        var handlerType = typeof(Action<>).MakeGenericType(argsType);
                        var handlerInvokerDelegate = handlerMethod.IsStatic ?
                                                     Delegate.CreateDelegate(typeof(Action), null, handlerMethod) :
                                                     Delegate.CreateDelegate(typeof(Action), obj, handlerMethod);

                        var proxy = new HandlerProxy(new DelegateReference(handlerInvokerDelegate, true));
                        var proxyHandlerMethod = typeof(HandlerProxy).GetMethod("Handler").MakeGenericMethod(argsType);
                        var handlerDelegate = Delegate.CreateDelegate(handlerType, proxy, proxyHandlerMethod);

                        Subscribe(e, handlerType, handlerDelegate, handlerInfo);
                    }
                    else if (handlerParameters.Count() == 1 && handlerParameters.Single().ParameterType == argsType)
                    {
                        var handlerType = typeof(Action<>).MakeGenericType(argsType);
                        var handlerDelegate = handlerMethod.IsStatic ?
                                              Delegate.CreateDelegate(handlerType, null, handlerMethod) :
                                              Delegate.CreateDelegate(handlerType, obj, handlerMethod);

                        Subscribe(e, handlerType, handlerDelegate, handlerInfo);
                    }
                    else
                    {
                        throw new Exception($"{GetMemberBasicInfo(obj, handlerMethod)} : \n " +
                                            $"Method must either contain no parameters or 1 single args parameter of the type the event is associated with : {argsType.Name}");
                    }
                }
            }
        }

        public void Subscribe(object e, Type handlerActionType, Delegate handlerDelegate, HandlesAttribute handlerInfo)
        {
            if (!handlerInfo.IsKeepSubscriberReferenceAliveSet &&
                   !handlerInfo.IsThreadOptionSet)
            {
                var subscribeMethod = e.GetType().GetMethod("Subscribe", new Type[] { handlerActionType });
                var subscribeDelegateType = typeof(Func<,>).MakeGenericType(new Type[] { handlerActionType, typeof(SubscriptionToken) });
                var subscribeDelegate = Delegate.CreateDelegate(subscribeDelegateType, e, subscribeMethod);

                subscribeDelegate.DynamicInvoke(new object[] { handlerDelegate });
            }

            else if (!handlerInfo.IsKeepSubscriberReferenceAliveSet &&
                     handlerInfo.IsThreadOptionSet)
            {
                var subscribeMethod = e.GetType().GetMethod("Subscribe", new Type[] { handlerActionType, typeof(ThreadOption) });
                var subscribeDelegateType = typeof(Func<,,>).MakeGenericType(new Type[] { handlerActionType, typeof(ThreadOption), typeof(SubscriptionToken) });
                var subscribeDelegate = Delegate.CreateDelegate(subscribeDelegateType, e, subscribeMethod);

                subscribeDelegate.DynamicInvoke(new object[] { handlerDelegate, handlerInfo.ThreadOption });

            }

            else if (handlerInfo.IsKeepSubscriberReferenceAliveSet &&
                   !handlerInfo.IsThreadOptionSet)
            {
                var subscribeMethod = e.GetType().GetMethod("Subscribe", new Type[] { handlerActionType, typeof(bool) });
                var subscribeDelegateType = typeof(Func<,,>).MakeGenericType(new Type[] { handlerActionType, typeof(bool), typeof(SubscriptionToken) });
                var subscribeDelegate = Delegate.CreateDelegate(subscribeDelegateType, e, subscribeMethod);

                subscribeDelegate.DynamicInvoke(new object[] { handlerDelegate, handlerInfo.KeepSubscriberReferenceAlive });
            }
            else
            {
                var subscribeMethod = e.GetType().GetMethod("Subscribe", new Type[] { handlerActionType, typeof(ThreadOption), typeof(bool) });
                var subscribeDelegateType = typeof(Func<,,,>).MakeGenericType(new Type[] { handlerActionType, typeof(ThreadOption), typeof(bool), typeof(SubscriptionToken) });
                var subscribeDelegate = Delegate.CreateDelegate(subscribeDelegateType, e, subscribeMethod);

                subscribeDelegate.DynamicInvoke(new object[] { handlerDelegate, handlerInfo.ThreadOption, handlerInfo.KeepSubscriberReferenceAlive });
            }
        }

        public string GetMemberBasicInfo(object obj, MethodInfo method)
        {
            return $"Handle Event exception in {obj.GetType().Name}, Method {method.Name}";
        }

        public void Teardown(object obj)
        {
            // TODO : Implement Unsubscription Mechanism
        }
    }

    public class HandlerProxy
    {
        private static readonly List<HandlerProxy> proxys;
        private static void CleanUp(object sender, EventArgs e)
        {
            lock (proxys)
            {
                var newList = proxys.Where(p => p.RealHandler.Target != null).ToList();
                proxys.Clear();
                proxys.AddRange(newList);
            }
        }

        static HandlerProxy()
        {
            proxys = new List<HandlerProxy>();
            if (Application.Current != null)
            {
                new DispatcherTimer(new TimeSpan(0, 0, 0, 15), DispatcherPriority.ApplicationIdle, CleanUp, Application.Current.Dispatcher).Start();
            }
        }

        public HandlerProxy(DelegateReference realHandler)
        {
            lock (proxys)
            {
                proxys.Add(this);
            }
            this.RealHandler = realHandler;
        }

        protected DelegateReference RealHandler { get; private set; }

        public void Handler<T>(T arg)
        {
            var hdl = (Action)this.RealHandler.Target;
            if (hdl != null)
            {
                hdl();
            }
        }

        public void HandlerWithConversion<TSource, TDestination>(TSource arg)
           where TSource : TDestination
        {
            var hdl = (Action<TDestination>)this.RealHandler.Target;
            if (hdl != null)
            {
                hdl(arg);
            }
        }
    }
}
