using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Common;
using Quantum.Utils;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class AutoInvalidateOnEvent : IAssertable, ICommandMetadata, IMultiCommandMetadata, ISubCommandMetadata, IToolBarMetadata, IStaticPanelMetadata, IDynamicPanelMetadata
    {
        public Type EventType { get; private set; }
        public AutoInvalidateOnEvent(Type eventType)
        {
            EventType = eventType;
        }

        [DebuggerHidden]
        public void Assert(string objName = null)
        {
            if(EventType == null) {
                throw new Exception($"Error : {objName ?? String.Empty} contains an AutoInvalidateOnEvent metadata definition that has a null event type.");
            }
            
            if(!EventType.IsSubclassOfRawGeneric(typeof(CompositePresentationEvent<>)))
            {
                throw new Exception($"Error : {objName ?? String.Empty} contains an AutoInvalidateOnEvent metadata definition that does not have a valid event type. " +
                    $"The type of the event must extend CompositePresentationEvent<TPayload>");
            }
        }
    }
}
