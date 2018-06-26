using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Utils;
using System;

namespace Quantum.Services
{
    public abstract class SelectionBase<T> : EventBase
    {
        private ThreadSyncScope BlockNotificationsScope { get; set; } = new ThreadSyncScope();

        public SelectionBase(IObjectInitializationService initSvc)
        {
            initSvc.Initialize(this);
            BlockNotificationsScope.OnAllScopesEnd += (sender, e) => OnAllBlockingScopesEnd();
        }

        public SelectionBase(IObjectInitializationService initSvc, T defaultValue, bool raiseOnDefaultValueSet = false)
            : this(initSvc)
        {
            if(raiseOnDefaultValueSet) {
                Value = defaultValue;
            }
            else {
                internalValue = defaultValue;
            }
        }

        private T internalValue;
        public T Value
        {
            get
            {
                return internalValue;
            }
            set
            {
                internalValue = value;
                Raise();
            }
        }

        #region Raise

        private volatile bool RequestDuringBlock = false;

        public IDisposable BeginBlockingNotifications() {
            return BlockNotificationsScope.BeginThreadScope();
        }

        private void OnAllBlockingScopesEnd() {
            if(RequestDuringBlock) {
                Raise();
            }
        }

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

        public void ForcePublish() {
            InternalPublish(this);
        }

        protected override void InternalPublish(params object[] arguments)
        {
            OnSelectedValueChanging();
            base.InternalPublish(arguments);
            OnSelectedValueChanged();
        }

        protected virtual void OnSelectedValueChanging() { }
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

        public SubscriptionToken Subscribe(Action<SelectionBase<T>> action)
        {
            return Subscribe(action, ThreadOption.UIThread);
        }

        public SubscriptionToken Subscribe(Action<SelectionBase<T>> action, ThreadOption threadOption)
        {
            return Subscribe(action, threadOption, false);
        }

        public SubscriptionToken Subscribe(Action<SelectionBase<T>> action, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            IDelegateReference actionReference = new DelegateReference(action, keepSubscriberReferenceAlive);
            return Subscribe(actionReference, threadOption, keepSubscriberReferenceAlive);
        }

        public SubscriptionToken Subscribe(IDelegateReference actionReference, ThreadOption threadOption)
        {
            return Subscribe(actionReference, threadOption, false);
        }

        public SubscriptionToken Subscribe(IDelegateReference actionReference, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
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
