using System;

namespace Quantum.Command
{
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class AutoInvalidateOnEvent : ICommandMetadata, IMultiMenuMetadata
    {
        public Type EventType { get; private set; }
        public AutoInvalidateOnEvent(Type eventType)
        {
            EventType = eventType;
        }
    }
}
