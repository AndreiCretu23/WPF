using System;
using System.Windows.Threading;

namespace Quantum.Utils
{
    public class ThreadSyncScope
    {
        public bool IsInScope { get; private set; }
        private Dispatcher OwnerDispatcher { get; set; }

        public EventHandler OnScopeBegin { get; set; }
        public EventHandler OnScopeEnd { get; set; }
        public EventHandler OnAllScopesEnd { get; set; }

        private volatile int ThreadScopeCount = 0;

        public ThreadSyncScope() {
            OwnerDispatcher = Dispatcher.CurrentDispatcher;
        }

        public IDisposable BeginThreadScope()
        {
            return new ThreadSyncScopeHelper(this);
        }

        private class ThreadSyncScopeHelper : IDisposable
        {
            private ThreadSyncScope SyncScope { get; set; }
            private bool WasDisposed { get; set; }

            public ThreadSyncScopeHelper(ThreadSyncScope syncScope)
            {
                SyncScope = syncScope;

                SyncScope.OwnerDispatcher.Invoke(() =>
                {
                    SyncScope = syncScope;
                    SyncScope.ThreadScopeCount++;
                    SyncScope.IsInScope = true;
                    SyncScope.OnScopeBegin?.Invoke(this, new EventArgs());
                });
            }

            public void Dispose()
            {
                if(!WasDisposed) {
                    SyncScope.OwnerDispatcher.Invoke(() =>
                    {
                        SyncScope.OnScopeEnd?.Invoke(this, new EventArgs());
                        SyncScope.ThreadScopeCount--;
                        if (SyncScope.ThreadScopeCount == 0)
                        {
                            SyncScope.IsInScope = false;
                            SyncScope.OnAllScopesEnd?.Invoke(this, new EventArgs());
                        }
                    });
                    
                }
                WasDisposed = true;
            }
        }

    }
}
