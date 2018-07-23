using System;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class AutoInvalidateOnEvent : ICommandMetadata, IMultiMenuMetadata, IToolBarMetadata
    {
        public Type EventType { get; private set; }
        public AutoInvalidateOnEvent(Type eventType)
        {
            EventType = eventType;
        }
    }
}
