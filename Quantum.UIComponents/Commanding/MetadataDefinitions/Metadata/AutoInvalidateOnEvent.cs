using System;

namespace Quantum.Command
{
    public class AutoInvalidateOnEvent : ICommandMetadata
    {
        public bool SupportsMultiple { get { return true; } }
        public Type EventType { get; private set; }
        public AutoInvalidateOnEvent(Type eventType)
        {
            EventType = eventType;
        }
    }
}
