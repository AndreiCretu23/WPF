//########################################################################
// (C) Socionext Embedded Software Austria GmbH (SESA)
// All rights reserved.
// -----------------------------------------------------
// This document contains proprietary information belonging to 
// Socionext Embedded Software Austria GmbH (SESA)
// Passing on and copying of this document, use and communication 
// of its contents is not permitted without prior written authorization.
//########################################################################
using Quantum.Events;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Quantum.UIComponents
{
    internal class LongOperationService : ServiceBase, ILongOperationService
    {
        public LongOperationService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            LastPosRect = new Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        }

        #region External

        [DllImport("User32.dll")]
        private static extern void DisableProcessWindowsGhosting();

        #endregion External

        [Handles(typeof(UILoadedEvent))]
        public void EnableWatchDog()
        {
            DisableProcessWindowsGhosting();
            LastUpdate = DateTime.Now;
            var wnd = Application.Current.MainWindow;
            if (wnd == null) return;

            LastPosRect = new Rect(0, 0, SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);

            LongOperationThread = new Thread(LongOperationThreadMethod);
            LongOperationThread.SetApartmentState(ApartmentState.STA);
            LongOperationThread.Start();
            Application.Current.Dispatcher.ShutdownStarted += (s, e) =>
               Dispatcher.FromThread(LongOperationThread).
                  IfNotNull(_ => _.BeginInvokeShutdown(DispatcherPriority.Send));
        }

        DateTime LastUpdate;
        public DispatcherTimer WatchdogTimer;

        public Thread LongOperationThread { get; private set; }
        public void LongOperationThreadMethod()
        {
            LastUpdate = DateTime.Now;
            
            WatchdogTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 1), DispatcherPriority.Send, WatchdogTimerMethod, Dispatcher.CurrentDispatcher);
            WatchdogTimer.Start();
            try
            {
                Dispatcher.Run();
            }
            catch { }
        }

        private readonly TimeSpan LongOpWait = new TimeSpan(0, 0, 0, 2, 500);
        private readonly TimeSpan ResendInterval = new TimeSpan(0, 0, 0, 0, 500);
        private Rect LastPosRect;
        private void UiWatchdogTimerMethod()
        {
            LastUpdate = DateTime.Now;

            var wnd = Application.Current.MainWindow;
            if (wnd != null)
            {
                LastPosRect = new Rect(wnd.Left, wnd.Top, wnd.ActualWidth, wnd.ActualHeight);
            }
        }

        private void WatchdogTimerMethod(object sender, EventArgs args)
        {
            var now = DateTime.Now;
            if (now - LastUpdate > ResendInterval && Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(UiWatchdogTimerMethod), DispatcherPriority.Send);
            }

            if (now - LastUpdate > LongOpWait && !IsLongOpDialogDisplaying)
            {
                Mouse.PrimaryDevice.Synchronize();
                if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed) return;
                try
                {
                    ShowLongOperationDialog();
                }
                catch { }
            }
        }
        
        private readonly object syncRoot = new object();
        public bool IsLongOpDialogDisplaying { get; private set; }
        
        public bool ShowLongOperationDialog()
        {
            lock (syncRoot)
            {
                if (IsLongOpDialogDisplaying)
                {
                    return false;
                }
                var disp = new LongOpDispatcher
                {
                    LongOpThreadDispatcher = Dispatcher.FromThread(LongOperationThread),
                    UIDispatcher = Application.Current.Dispatcher,
                    WindowCreator = () =>
                    {
                        var wnd = new LongOperationView()
                        {
                            DataContext = new LongOperationViewModel(InitializationService),
                            WindowStartupLocation = WindowStartupLocation.Manual,
                        };
                        wnd.Top = LastPosRect.Top + LastPosRect.Height / 2 - wnd.Height / 2;
                        wnd.Left = LastPosRect.Left + LastPosRect.Width / 2 - wnd.Width / 2;
                        return wnd;
                    }
                };
                IsLongOpDialogDisplaying = true;

                disp.OperationFinished += () => { lock (syncRoot) { IsLongOpDialogDisplaying = false; } };
                disp.StartOperation();
                return true;
            }
        }
    }
    
}
