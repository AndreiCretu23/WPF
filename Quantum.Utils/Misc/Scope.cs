using System;
using System.Windows.Threading;

namespace Quantum.Utils
{
    public class Scope
    {
        public bool IsInScope { get; private set; }
        private Dispatcher OwnerDispatcher { get; set; }

        public EventHandler OnScopeBegin { get; set; }
        public EventHandler OnScopeEnd { get; set; }
        public EventHandler OnAllScopesEnd { get; set; }

        private volatile int ScopeCount = 0;

        public Scope() {
            OwnerDispatcher = Dispatcher.CurrentDispatcher;
        }

        public IDisposable BeginScope()
        {
            return new ScopeHelper(this);
        }

        private class ScopeHelper : IDisposable
        {
            private Scope SyncScope { get; set; }
            private bool WasDisposed { get; set; }

            public ScopeHelper(Scope syncScope)
            {
                SyncScope = syncScope;

                SyncScope.OwnerDispatcher.Invoke(() =>
                {
                    SyncScope = syncScope;
                    SyncScope.ScopeCount++;
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
                        SyncScope.ScopeCount--;
                        if (SyncScope.ScopeCount == 0)
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
