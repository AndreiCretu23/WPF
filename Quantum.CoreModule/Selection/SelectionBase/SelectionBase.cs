using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Utils;
using System;

namespace Quantum.Services
{
    /// <summary>
    /// The base class for all selections. A selection is an event-wrapper class over an object.
    /// It holds a reference to an object inside it's value property and whenever that value changes, 
    /// the event raises itself. For implementations, see SingleSelection and MultipleSelection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SelectionBase<T> : EventBase, ISelection
    {
        private Scope BlockNotificationsScope { get; set; } = new Scope();

        public SelectionBase()
        {
            BlockNotificationsScope.OnAllScopesEnd += (sender, e) => OnAllBlockingScopesEnd();
        }
        
        /// <summary>
        /// The value currently stored inside the instance of the selection.
        /// </summary>
        public abstract T Value { get; set; }

        object ISelection.SelectedObject => Value;
        Type ISelection.SelectionType => typeof(T);


        #region Raise

        private volatile bool RequestDuringBlock = false;

        /// <summary>
        /// Begins a scope in which all notifications are disabled. If the value changes inside this scope, 
        /// the event will not raise itself. When the scope ends, if the value changed during it, the event will raise itself.
        /// </summary>
        /// <returns></returns>
        public IDisposable BeginBlockingNotifications() {
            return BlockNotificationsScope.BeginScope();
        }

        private void OnAllBlockingScopesEnd() {
            if(RequestDuringBlock) {
                Raise();
            }
        }

        /// <summary>
        /// Raises the event if notifications are not blocked.
        /// </summary>
        protected void Raise() {
            if(BlockNotificationsScope.IsInScope) {
                RequestDuringBlock = true;
            }
            else {
                if (RequestDuringBlock) {
                    RequestDuringBlock = false;
                }
                InternalPublish(this);
            }
        }

        /// <summary>
        /// Publishes the event as if the selection changed.
        /// </summary>
        public void ForcePublish() {
            InternalPublish(this);
        }

        /// <summary>
        /// Notifies, internally, that the selection has changed, then publishes the event, and then, notifies 
        /// internally that the selection has changed and that the event has been published and handled by the application.
        /// </summary>
        /// <param name="arguments"></param>
        protected override void InternalPublish(params object[] arguments)
        {
            OnSelectedValueChanging();
            base.InternalPublish(arguments);
            OnSelectedValueChanged();
        }
        
        /// <summary>
        /// A method that gets called after the selection changed, but before the event is published.
        /// </summary>
        protected virtual void OnSelectedValueChanging() { }

        /// <summary>
        /// A method that gets called after the selection changed and the event is published.
        /// </summary>
        protected virtual void OnSelectedValueChanged() { }

        #endregion Raise


        #region Subscribe

        private IDispatcherFacade uiDispatcher;
        private IDispatcherFacade UIDispatcher
        {
            get
            {
                if (uiDispatcher == null)
                {
                    uiDispatcher = new DefaultDispatcher();
                }
                return uiDispatcher;
            }
        }

        /// <summary>
        /// Subscribes the given delegate to the SelectionChanging event. The delegate will be invoked on the PublisherThread.
        /// </summary>
        /// <param name="action">The event handler</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe(Action<SelectionBase<T>> action)
        {
            return Subscribe(action, ThreadOption.PublisherThread);
        }

        /// <summary>
        /// Subscribes the given delegate to the SelectionChanging event. The delegate will be invoked on the specified thread.
        /// </summary>
        /// <param name="action">The event handler</param>
        /// <param name="threadOption">The thread on which the event handler will be invoked</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe(Action<SelectionBase<T>> action, ThreadOption threadOption)
        {
            return Subscribe(action, threadOption, true);
        }

        /// <summary>
        /// Subscribes the given delegate to the SelectionChanging event. The delegate will be invoked on the specified thread.
        /// </summary>
        /// <param name="action">The event handler</param>
        /// <param name="threadOption">The thread on which the event handler will be invoked</param>
        /// <param name="keepSubscriberReferenceAlive">A parameter which indicates if the subscription should be kept alive or not.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe(Action<SelectionBase<T>> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            return Subscribe(actionReference, threadOption);
        }
        
        /// <summary>
        /// Subscribes the given delegate reference to the SelectionChanging event. The delegate will be invoked on the specified thread.
        /// </summary>
        /// <param name="actionReference"></param>
        /// <param name="threadOption"></param>
        /// <returns></returns>
        public SubscriptionToken Subscribe(IDelegateReference actionReference, ThreadOption threadOption)
        {
            IDelegateReference filterReference =
               new DelegateReference(new Predicate<SelectionBase<T>>(_ => true), true);

            EventSubscription<SelectionBase<T>> subscription;
            switch (threadOption)
            {
                case ThreadOption.PublisherThread:
                    subscription = new EventSubscription<SelectionBase<T>>(actionReference, filterReference);
                    break;
                case ThreadOption.BackgroundThread:
                    subscription = new BackgroundEventSubscription<SelectionBase<T>>(actionReference, filterReference);
                    break;
                case ThreadOption.UIThread:
                    subscription = new DispatcherEventSubscription<SelectionBase<T>>(actionReference, filterReference, UIDispatcher);
                    break;
                default:
                    throw new NotImplementedException("Thread Option not available."); // Impossible to happen
            }
            return base.InternalSubscribe(subscription);
        }
        
        #endregion Subscribe
        
    }
}
