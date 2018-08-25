using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    [SuppressMessage("Microsoft.Design", "IDE0019")]
    internal static class AvalonProperties
    {

        public static readonly RoutedEvent UserNotifyEvent =
          EventManager.RegisterRoutedEvent(
          "UserNotify",
          RoutingStrategy.Direct,
          typeof(RoutedEventHandler),
          typeof(AvalonProperties));

        public static void AddUserNotifyHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.AddHandler(UserNotifyEvent, handler);
            }
        }

        public static void RemoveUserNotifyHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.RemoveHandler(UserNotifyEvent, handler);
            }
        }

        public static readonly RoutedEvent UserNotifyClearNotificationsEvent =
        EventManager.RegisterRoutedEvent(
        "UserNotifyClearNotifications",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(AvalonProperties));

        public static void AddUserNotifyClearNotificationsEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.AddHandler(UserNotifyClearNotificationsEvent, handler);
            }
        }

        public static void RemoveUserNotifyClearNotificationsEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.RemoveHandler(UserNotifyClearNotificationsEvent, handler);
            }
        }

        public static readonly RoutedEvent UserNotifyBubbleEvent =
            EventManager.RegisterRoutedEvent(
            "UserNotifyBubble",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(AvalonProperties));

        public static void AddUserNotifyBubbleHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.AddHandler(UserNotifyBubbleEvent, handler);
            }
        }

        public static void RemoveUserNotifyBubbleHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.RemoveHandler(UserNotifyBubbleEvent, handler);
            }
        }

        public static readonly RoutedEvent UserNotifyErrorBubbleEvent =
         EventManager.RegisterRoutedEvent(
         "UserNotifyErrorBubble",
         RoutingStrategy.Bubble,
         typeof(RoutedEventHandler),
         typeof(AvalonProperties));

        public static void AddUserNotifyErrorBubbleHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.AddHandler(UserNotifyErrorBubbleEvent, handler);
            }
        }

        public static void RemoveUserNotifyErrorBubbleHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.RemoveHandler(UserNotifyErrorBubbleEvent, handler);
            }
        }

        public static readonly RoutedEvent UserNotifyWarningBubbleEvent =
        EventManager.RegisterRoutedEvent(
        "UserNotifyWarningBubble",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(AvalonProperties));

        public static void AddUserNotifyWarningBubbleEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.AddHandler(UserNotifyWarningBubbleEvent, handler);
            }
        }

        public static void RemoveUserNotifyWarningBubbleEventHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement ui = d as UIElement;
            if (ui != null)
            {
                ui.RemoveHandler(UserNotifyWarningBubbleEvent, handler);
            }
        }

        public static bool GetIsEventTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEventTargetProperty);
        }

        public static void SetIsEventTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEventTargetProperty, value);
        }
        
        public static readonly DependencyProperty IsEventTargetProperty =
            DependencyProperty.RegisterAttached("IsEventTarget", typeof(bool), typeof(AvalonProperties), new PropertyMetadata(false, (s, e) =>
            {
                var dockManager = s.FindVisualParentOfType<DockingManager>();
                if (dockManager == null) return;
                var eventTargets = GetEventTargets(dockManager);

                if (eventTargets == null)
                {
                    dockManager.AddHandler(UserNotifyEvent, new RoutedEventHandler(DockManagerByTypeRouter));
                    SetEventTargets(dockManager, eventTargets = new Dictionary<string, FrameworkElement>());
                }

                var anchorableContent = s.IfIs((FrameworkElement _) => _.DataContext) as LayoutAnchorable;
                if (anchorableContent == null) return;

                eventTargets[anchorableContent.ContentId] = s as FrameworkElement;

            }));

        private static void DockManagerByTypeRouter(object sender, RoutedEventArgs args)
        {
            var dockManager = sender as DockingManager;
            if (dockManager == null) return;

            var anchorableSource = args.OriginalSource as LayoutAnchorable;
            if (anchorableSource == null) return;

            var eventTargets = GetEventTargets(dockManager);
            if (eventTargets == null) return;

            var outpuRoutedArgs = args as OutputRoutedEventArgs;

            if (outpuRoutedArgs != null && outpuRoutedArgs.EventType == OutputEventType.Clear)
            {
                eventTargets.
                   GetValueOrDefault(anchorableSource.ContentId).
                   IfNotNull(_ => _.RaiseEvent(new RoutedEventArgs(UserNotifyClearNotificationsEvent, sender)));

                return;
            }

            if (outpuRoutedArgs != null && outpuRoutedArgs.EventType == OutputEventType.Error)
            {
                eventTargets.
                   GetValueOrDefault(anchorableSource.ContentId).
                   IfNotNull(_ => _.RaiseEvent(new RoutedEventArgs(UserNotifyErrorBubbleEvent, sender)));

                return;
            }

            if (outpuRoutedArgs != null && outpuRoutedArgs.EventType == OutputEventType.Warning)
            {
                eventTargets.
                   GetValueOrDefault(anchorableSource.ContentId).
                   IfNotNull(_ => _.RaiseEvent(new RoutedEventArgs(UserNotifyWarningBubbleEvent, sender)));

                return;
            }

            if (outpuRoutedArgs != null && outpuRoutedArgs.EventType == OutputEventType.Info)
            {
                return;
            }

            eventTargets.
            GetValueOrDefault(anchorableSource.ContentId).
            IfNotNull(_ => _.RaiseEvent(new RoutedEventArgs(UserNotifyBubbleEvent, sender)));
        }

        public static Dictionary<string, FrameworkElement> GetEventTargets(DependencyObject obj)
        {
            return (Dictionary<string, FrameworkElement>)obj.GetValue(EventTargetsProperty);
        }

        public static void SetEventTargets(DependencyObject obj, Dictionary<string, FrameworkElement> value)
        {
            obj.SetValue(EventTargetsProperty, value);
        }

        public static readonly DependencyProperty EventTargetsProperty =
           DependencyProperty.RegisterAttached("EventTargets", typeof(Dictionary<string, FrameworkElement>), typeof(AvalonProperties), new PropertyMetadata(null));




        public static bool GetBringIntoViewOnDragOver(DependencyObject obj)
        {
            return (bool)obj.GetValue(BringIntoViewOnDragOverProperty);
        }

        public static void SetBringIntoViewOnDragOver(DependencyObject obj, bool value)
        {
            obj.SetValue(BringIntoViewOnDragOverProperty, value);
        }

        public static readonly DependencyProperty BringIntoViewOnDragOverProperty =
            DependencyProperty.RegisterAttached("BringIntoViewOnDragOver", typeof(bool), typeof(AvalonProperties), new PropertyMetadata(false, BringIntoViewOnDragOver.OnPropertyChanged));


    }

    internal enum OutputEventType
    {
        Info = 1,
        Warning = 2,
        Error = 3,
        Clear = 4
    }

    internal class OutputEventArgs : EventArgs
    {
        internal static OutputEventArgs Error = new OutputEventArgs(OutputEventType.Error);
        internal static OutputEventArgs Warning = new OutputEventArgs(OutputEventType.Warning);
        internal static OutputEventArgs Info = new OutputEventArgs(OutputEventType.Info);
        internal static OutputEventArgs Clear = new OutputEventArgs(OutputEventType.Clear);

        internal OutputEventArgs(OutputEventType eventType)
        {
            this.EventType = eventType;
        }

        internal OutputEventType EventType { get; private set; }
    }

    internal class OutputRoutedEventArgs : RoutedEventArgs
    {
        internal OutputRoutedEventArgs(RoutedEvent routedevent, object source, OutputEventType eventtype) : base(routedevent, source)
        {
            this.EventType = eventtype;
        }

        internal OutputEventType EventType { get; private set; }
    }

    internal class BringIntoViewOnDragOver
    {
        public static void OnPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)dependencyObject;
            if ((bool)e.NewValue)
            {
                fe.AllowDrop = true;

                fe.DragEnter += (s, ee) => DragFocus.Reset(s.IfIs((FrameworkElement _) => _.DataContext) as LayoutAnchorable);
                fe.DragLeave += (s, ee) => DragFocus.Stop();
            }
        }

        /// <summary>
        /// Used to activate a panel after the a drag operation hovers over for a little while
        /// </summary>
        private static DelayedExecutor<LayoutAnchorable> dragFocus;
        private static DelayedExecutor<LayoutAnchorable> DragFocus
        {
            get
            {
                if (dragFocus == null)
                {
                    dragFocus = new DelayedExecutor<LayoutAnchorable>(Dispatcher.CurrentDispatcher, dock =>
                    {
                        dock.Parent.
                           CaseType((LayoutAnchorablePane _) => _.SelectedContentIndex = _.Children.IndexOf(dock)).
                           CaseType((LayoutDocumentPane _) => _.SelectedContentIndex = _.Children.IndexOf(dock));
                    });
                }
                return dragFocus;
            }
        }
    }

    public class DelayedExecutor<T>
      where T : class
    {
        public T Target { get; set; }
        private readonly DispatcherTimer timer;
        private readonly Dispatcher dispatcher;
        private readonly Action<T> onTimeout;
        private readonly bool ensureMinExecutionTime;
        private DateTime lastExecutionTime;
        private readonly bool alwaysReset;
        private readonly TimeSpan interval;

        public DelayedExecutor(Dispatcher dispatcher, Action<T> onTimeout, DispatcherPriority priority = DispatcherPriority.Normal, TimeSpan interval = default(TimeSpan),
           bool ensureMinExecutionTime = false,
           bool alwaysReset = false)
        {
            this.ensureMinExecutionTime = ensureMinExecutionTime;
            this.alwaysReset = alwaysReset;

            this.interval = interval = interval == TimeSpan.Zero ? new TimeSpan(0, 0, 1) : interval;

            this.timer = new DispatcherTimer(interval, priority, (s, e) => this.OnTimeout(), dispatcher);
            this.timer.Stop();

            this.dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;

            WeakEventListenerManager<EventHandler>.Add(
               this,
               this.dispatcher,
               weakListener => (s, e) => weakListener.ActOnTarget(t => t.Dispatcher_ShutdownStarted(s, e)),
               (s, h) => s.ShutdownStarted += h,
               (s, h) => s.ShutdownStarted -= h);

            this.onTimeout = onTimeout;
        }

        void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            this.Stop();
        }

        public void Reset()
        {
            this.Reset(this.Target);
        }

        public void Reset(T newTarget)
        {
            if (newTarget == this.Target && !alwaysReset) return;

            this.timer.Stop();

            this.Target = newTarget;
            if (ensureMinExecutionTime && DateTime.Now - this.lastExecutionTime > this.interval)
            {
                this.OnTimeout();
            }
            else
            {
                if (newTarget != null)
                {
                    this.timer.Start();
                }
            }
        }

        public void Stop()
        {
            this.timer.Stop();
            this.Target = null;
        }
        void OnTimeout()
        {
            if (this.Target != null)
            {
                this.dispatcher.BeginInvoke(new Action(() => { if (this.Target != null) this.onTimeout(this.Target); }));
            }
            this.timer.Stop();
            this.lastExecutionTime = DateTime.Now;
        }
    }

    public class WeakEventListenerManager<TEventHandler> : WeakEventListenerManager
    {
        // Summary:
        //     Add a new event handler. The event is subscribed before the function returns,
        //     and is unsubscribed once the target becomes garbage collected.
        //     These event listeners are weak in both target and source, meaning that no hard
        //     reference is maintained to either of them.
        //
        // Parameters:
        //   target:
        //     The target of the message generated by the event. The observer. Can't be null.
        //   source:
        //     The source of the event. The observable. Can't be null.
        //   handler:
        //     Function that generates an event handler. This function recieves a reference to
        //     the WeakEventListener, to have the means to access the target in a way that doesn't
        //     create a hard reference to it.
        //     Typicaly looks like: weakListener=>(s, e)=>weakListener.ActOnTarget(t=>t.HandleEvent(s, e))
        //   subscribe:
        //     Subscribe to event. First parameter is the source (as long as it is alive). The second parameter
        //     is the handler, generated by the "handler" method.
        //     Typicaly looks like: (s, h)=>s.EventHandler+=h
        //   unsubscribe:
        //     Unsubscribe from event. First parameter is the source (as long as it is alive). The second parameter
        //     is the handler, generated by the "handler" method.
        //     Typicaly looks like: (s, h)=>s.EventHandler-=h
        //
        // Type parameters:
        //   TEventTarget:
        //     The type of "target".
        //   TEventSource:
        //     The type of "source".
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

        // Summary:
        //     Add a new event handler. The event is subscribed before the function returns,
        //     and is unsubscribed once the target becomes garbage collected.
        //     These event listeners are weak in both target and source, meaning that no hard
        //     reference is maintained to either of them.
        //     In addition allow hard binding a data object, that will be kept alive as long as
        //     the listener survives (i.e. either source or target is alive).
        //
        // Parameters:
        //   target:
        //     The target of the message generated by the event. The observer. Can't be null.
        //   source:
        //     The source of the event. The observable. Can't be null.
        //   data:
        //     Data made available to the handler, through WeakEventListener.Data.
        //   handler:
        //     Function that generates an event handler. This function recieves a reference to
        //     the WeakEventListener, to have the means to access the target in a way that doesn't
        //     create a hard reference to it.
        //     Typicaly looks like: weakListener=>(s, e)=>weakListener.ActOnTarget(t=>t.HandleEvent(s, e))
        //   subscribe:
        //     Subscribe to event. First parameter is the source (as long as it is alive). The second parameter
        //     is the handler, generated by the "handler" method.
        //     Typicaly looks like: (s, h)=>s.EventHandler+=h
        //   unsubscribe:
        //     Unsubscribe from event. First parameter is the source (as long as it is alive). The second parameter
        //     is the handler, generated by the "handler" method.
        //     Typicaly looks like: (s, h)=>s.EventHandler-=h
        //
        // Type parameters:
        //   TEventTarget:
        //     The type of "target".
        //   TEventSource:
        //     The type of "source".
        //   TEventData:
        //     The type of "data".
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
                // removed until we figure out how to do this in VS2015
                // throw new NotSupportedException("There shouldn't be any object capture in the delegates. Collect all needed resources in either source or target!");
            }

            Register(new WeakEventListener<TEventTarget, TEventSource, TEventData, TEventHandler>(target, source, data, handler, subscribe, unsubscribe));
        }
    }

    // Summary:
    //     Class that handles the lifetime of the WeakEventListeners. A timer action
    //     executes the clean-up periodically. Clean-up can also be forced through this interface.
    public class WeakEventListenerManager
    {
        private static readonly List<WeakEventListener> weakListeners;
        // Summary:
        //     Perform a blocking Clean-up operation.
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

    // Summary:
    //     Base class of all WeakEventListeners. Used by WeakEventListenerManager.
    //     No public interface.
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


    // Summary:
    //     Class that is instantiated by the manager to handle the state of the event binding.
    //     It provides an interface used by the event handler callback.
    //     Don't capture references to this class outside the "handler". That might lead to undefined behavior.
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    [SuppressMessage("Microsoft.Design", "IDE0019")]
    [SuppressMessage("Microsoft.Design", "IDE0044")]
    public class WeakEventListener<TEventTarget, TEventSource, TEventData, TEventHandler> : WeakEventListener
       where TEventTarget : class
       where TEventSource : class
    {
        // Summary:
        //     Use in handler to acces the target.
        //     Will only be called with non-null target.
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

        // Summary:
        //     Use in handler to acces bound data.
        public TEventData Data;

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

        private TEventHandler Handler;
        private Action<TEventSource, TEventHandler> UnsubscribeAction;

        internal override void Unsubscribe()
        {
            var source = base.Source.Target as TEventSource;
            if (source != null)
                this.UnsubscribeAction(source, this.Handler);
        }
    }
}
