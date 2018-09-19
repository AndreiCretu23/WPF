using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Quantum.UIComponents
{
    internal class LongOpDispatcher
    {
        private bool HasEnded;
        private bool IsShown;
        private DispatcherTimer FocusCheckTimer;
        public Dispatcher LongOpThreadDispatcher { get; set; }
        public Dispatcher UIDispatcher { get; set; }

        public event Action OperationFinished;

        public IntPtr ApplicationProcessId { get; set; }
        public Func<Window> WindowCreator { get; set; }
        private Window LongOpView;

        #region External

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr ProcessId);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetCurrentProcessId();

        #endregion External
        
        public void StartOperation()
        {
            ApplicationProcessId = GetCurrentProcessId();

            LongOpThreadDispatcher.Invoke(new Action(ShowWindow));
            EndLongOperation();
        }

        public void EndLongOperation()
        {
            UIDispatcher.BeginInvoke(
                  new Action(EndOperation),
                  DispatcherPriority.Render);
        }

        private void ActivateWindow()
        {
            lock (syncRoot)
            {
                var hwnd = GetForegroundWindow();
                if (hwnd == windowHandle) return; 

                GetWindowThreadProcessId(hwnd, out IntPtr activeWindowProcess);
                if (LongOpView != null && (ApplicationProcessId == activeWindowProcess || hwnd == IntPtr.Zero))
                {
                    LongOpView.Activate();
                }
            }
        }

        private IntPtr windowHandle;
        private void ShowWindow()
        {
            lock (syncRoot)
            {
                if (!HasEnded && !IsShown && !Application.Current.Dispatcher.HasShutdownStarted)
                {
                    IsShown = true;
                    helperWindow = new Window
                    {
                        WindowStyle = WindowStyle.ToolWindow,
                        Width = 0,
                        Height = 0,
                        Top = -100,
                        Left = -100,
                        ShowInTaskbar = false
                    };

                    helperWindow.Show();
                    LongOpView = WindowCreator();
                    LongOpView.Owner = helperWindow;
                    LongOpView.Show();
                    windowHandle = new WindowInteropHelper(LongOpView).Handle;
                    LongOpView.Activate();

                    FocusCheckTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100),
                       DispatcherPriority.Send,
                       (s, e) => ActivateWindow(),
                       LongOpThreadDispatcher);
                }
            }
        }
        private readonly object syncRoot = new object();
        private Window helperWindow;

        private void EndOperation()
        {
            LongOpThreadDispatcher.BeginInvoke(new Action(EndOperationImpl));
        }

        private void EndOperationImpl()
        {
            lock (syncRoot)
            {
                FocusCheckTimer?.Stop();
                if (IsShown)
                {
                    helperWindow.Close();
                    LongOpView.Close();
                }
                HasEnded = true;

                OperationFinished?.Invoke();
            }
        }
    }
}
