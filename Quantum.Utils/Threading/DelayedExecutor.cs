using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Quantum.Utils
{

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
    
}
