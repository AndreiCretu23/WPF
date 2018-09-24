using System;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used to indicate whether the UIDefinition owner is checkable or not. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class Checkable : IMainMenuMetadata, ISubMainMenuMetadata
    {
        public bool Value { get; private set; }
        public Checkable(bool isCheckable) {
            Value = isCheckable;
        }
        public Checkable(Func<bool> isCheckableGetter) {
            Value = isCheckableGetter();
        }
    }
}
