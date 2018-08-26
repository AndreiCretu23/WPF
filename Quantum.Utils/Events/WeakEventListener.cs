using System;
using System.Diagnostics.CodeAnalysis;

namespace Quantum.Utils
{
    public abstract class WeakEventListener
    {
        protected WeakReference Target { get; private set; }
        protected WeakReference Source { get; private set; }

        internal abstract void Unsubscribe();

        protected WeakEventListener(object target, object source)
        {
            this.Target = new WeakReference(target);
            this.Source = new WeakReference(source);
        }

        internal bool IsAlive
        {
            get
            {
                return this.Target.IsAlive && this.Source.IsAlive;
            }
        }
    }

    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    [SuppressMessage("Microsoft.Design", "IDE0019")]
    [SuppressMessage("Microsoft.Design", "IDE0044")]
    public class WeakEventListener<TEventTarget, TEventSource, TEventData, TEventHandler> : WeakEventListener
       where TEventTarget : class
       where TEventSource : class
    {
        public TEventData Data;
        private TEventHandler Handler;
        private Action<TEventSource, TEventHandler> UnsubscribeAction;

        internal WeakEventListener(
           TEventTarget target,
           TEventSource source,
           TEventData data,
           Func<WeakEventListener<TEventTarget, TEventSource, TEventData, TEventHandler>, TEventHandler> handler,
           Action<TEventSource, TEventHandler> subscribe,
           Action<TEventSource, TEventHandler> unsubscribe) :
           base(target, source)
        {
            this.Data = data;
            this.Handler = handler(this);
            this.UnsubscribeAction = unsubscribe;
            subscribe(source, this.Handler);
        }

        public void ActOnTarget(Action<TEventTarget> action)
        {
            var target = base.Target.Target as TEventTarget;
            if (target != null)
            {
                action(target);
            }
            else
            {
                this.Unsubscribe();
            }
        }


        internal override void Unsubscribe()
        {
            var source = base.Source.Target as TEventSource;
            if (source != null)
                this.UnsubscribeAction(source, this.Handler);
        }
    }

}
