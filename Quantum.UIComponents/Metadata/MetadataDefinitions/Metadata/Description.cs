using Quantum.Common;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used to attach a description to it's UIDefinition owner. 
    /// This metadata type is mandatory in any metadata collection that supports it. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class Description : IAssertable, IMainMenuMetadata, ISubMainMenuMetadata, IPanelMenuEntryMetadata
    {
        public string Value { get; private set; }
        public Description(string description)
        {
            Value = description;
        }
        public Description(Func<string> descriptionGetter)
        {
            Value = descriptionGetter();
        }

        [DebuggerHidden]
        public void Assert(string objName = null)
        {
            if(Value == null)
            {
                throw new Exception($"Error : {objName ?? String.Empty} contains a Description metadata definition that has a null value.");
            }
        }
    }
}
