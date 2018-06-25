using Microsoft.Practices.Composite.Events;
using Quantum.Utils;
using System;

namespace Quantum.Services
{
    public class SelectionBase : EventBase
    {
        protected ScopedValue<bool> BlockNotificationsScope { get; set; } = new ScopedValue<bool>();
        
        public SelectionBase()
        {
        }

        public bool AreNotificationsBlocked { get { return BlockNotificationsScope.Value; } }

        public IDisposable BeginBlockNotifications() {
            return BlockNotificationsScope.BeginValueScope(true);
        }

        protected void Raise() {
            if(BlockNotificationsScope.Value) {
                BlockNotificationsScope.OnScopeEnd += (sender, e) => InternalPublish(this);
            }
            else {
                InternalPublish(this);
            }
        }
        
        public void ForceRaise() {
            InternalPublish(this);
        }
    }
}
