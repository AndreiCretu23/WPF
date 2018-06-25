using System;

namespace Quantum.Utils
{
    public class ScopedValue<T>
    {
        public T Value { get; private set; }

        public ScopedValue() : this(default(T)) { }
        public ScopedValue(T defaultValue, EventHandler<ScopeChangedEventArgs<T>> onScopeBegin = null, EventHandler<ScopeChangedEventArgs<T>> onScopeEnd = null)
        {
            Value = defaultValue;

            onScopeBegin.IfNotNull(_ => this.OnScopeBegin += _);
            onScopeEnd.IfNotNull(_ => this.OnScopeEnd += _);
        }

        public IDisposable BeginValueScope(T value)
        {
            return new ScopedValueHelper<T>(this, value);
        }

        public event EventHandler<ScopeChangedEventArgs<T>> OnScopeBegin;
        public event EventHandler<ScopeChangedEventArgs<T>> OnScopeEnd;

        private class ScopedValueHelper<TInner> : IDisposable
        {
            readonly ScopedValue<TInner> parent;
            readonly TInner originalValue;
            public ScopedValueHelper(ScopedValue<TInner> parent, TInner value)
            {
                this.parent = parent;
                originalValue = parent.Value;
                parent.Value = value;
                this.parent.OnScopeBegin?.Invoke(this.parent, new ScopeChangedEventArgs<TInner>(this.parent, originalValue));
            }

            public void Dispose()
            {
                var endingValue = parent.Value;
                parent.Value = originalValue;
                this.parent.OnScopeEnd?.Invoke(this.parent, new ScopeChangedEventArgs<TInner>(this.parent, endingValue));
            }
        }
    }

    public class ScopeChangedEventArgs<T> : EventArgs
    {
        public ScopeChangedEventArgs(ScopedValue<T> scope, T value)
        {
            Value = value;
            Scope = scope;
        }

        public ScopedValue<T> Scope { get; private set; }
        public T Value { get; private set; }
    }
}
