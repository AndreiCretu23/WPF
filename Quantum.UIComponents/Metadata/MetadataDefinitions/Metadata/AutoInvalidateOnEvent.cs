using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Common;
using Quantum.Utils;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type hints the owner that an invalidation is required when the event of the specified type 
    /// in fired in the event aggregator instance of the application's container. The invalidation effect varies 
    /// depending on the owner of this metadata type. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
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
