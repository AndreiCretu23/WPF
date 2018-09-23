namespace Quantum.Services
{
    public class SingleSelectionCache<T>
    {
        public T Value { get; }
        public SingleSelectionCache(T value)
        {
            Value = value;
        }
    }
}
