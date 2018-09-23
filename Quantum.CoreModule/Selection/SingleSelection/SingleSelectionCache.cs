namespace Quantum.Services
{
    public class SingleSelectionCache<T> : ISingleSelectionCache
    {
        public T Value { get; }
        object ISingleSelectionCache.Value { get { return Value; } }

        public SingleSelectionCache(T value)
        {
            Value = value;
        }
    }
}
