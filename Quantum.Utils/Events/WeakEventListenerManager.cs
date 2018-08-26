using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Quantum.Utils
{
    public class WeakEventListenerManager
    {
        private static readonly List<WeakEventListener> weakListeners;

        public static void CleanUp()
        {
            lock (weakListeners)
            {
                var newList = weakListeners.Where(p =>
                {
                    bool isAlive = p.IsAlive;
                    if (!isAlive)
                    {
                        p.Unsubscribe();
                    }
                    return isAlive;
                }).ToList();

                weakListeners.Clear();
                weakListeners.AddRange(newList);
            }
        }

        protected static void Register(WeakEventListener listener)
        {
            lock (weakListeners)
            {
                weakListeners.Add(listener);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static WeakEventListenerManager()
        {
            weakListeners = new List<WeakEventListener>();
            if (Application.Current != null)
            {
                new DispatcherTimer(new TimeSpan(0, 0, 0, 1), DispatcherPriority.ApplicationIdle, CleanUp, Application.Current.Dispatcher).Start();
            }
        }

        private static void CleanUp(object sender, EventArgs e)
        {
            CleanUp();
        }
    }

    public class WeakEventListenerManager<TEventHandler> : WeakEventListenerManager
    {
        public static void Add<TEventTarget, TEventSource>(
           TEventTarget target,
           TEventSource source,
           Func<WeakEventListener<TEventTarget, TEventSource, object, TEventHandler>, TEventHandler> handler,
           Action<TEventSource, TEventHandler> subscribe,
           Action<TEventSource, TEventHandler> unsubscribe)
           where TEventTarget : class
           where TEventSource : class
        {
            Add<TEventTarget, TEventSource, object>(target, source, null, handler, subscribe, unsubscribe);
        }

        public static void Add<TEventTarget, TEventSource, TEventData>(
           TEventTarget target,
           TEventSource source,
           TEventData data,
           Func<WeakEventListener<TEventTarget, TEventSource, TEventData, TEventHandler>, TEventHandler> handler,
           Action<TEventSource, TEventHandler> subscribe,
           Action<TEventSource, TEventHandler> unsubscribe)
           where TEventTarget : class
           where TEventSource : class
        {
            target = target.AssertNotNull(nameof(target));
            handler = handler.AssertNotNull(nameof(handler));
            subscribe = subscribe.AssertNotNull(nameof(subscribe));
            unsubscribe = unsubscribe.AssertNotNull(nameof(unsubscribe));

            if (handler.Target != null || subscribe.Target != null || unsubscribe.Target != null)
            {
            }

            Register(new WeakEventListener<TEventTarget, TEventSource, TEventData, TEventHandler>(target, source, data, handler, subscribe, unsubscribe));
        }
    }

}
