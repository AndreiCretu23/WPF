using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

}
