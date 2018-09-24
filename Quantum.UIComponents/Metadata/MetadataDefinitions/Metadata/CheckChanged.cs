using Quantum.Common;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used to attach an action when the UIDefinition owner's "Check" state changes.
    /// Does nothing if the parent definition owner does not have a "Checkable(true)" metadata definition. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class CheckChanged : IAssertable, IMainMenuMetadata, ISubMainMenuMetadata
    {
        public Action<bool> OnCheckChanged { get; private set; }
        public CheckChanged(Action<bool> onCheckChanged)
        {
            OnCheckChanged = onCheckChanged;
        }

        [DebuggerHidden]
        public void Assert(string objName = null)
        {
            if(OnCheckChanged == null)
            {
                throw new Exception($"Error : {objName} contains a CheckChanged Metadata definition that has a null action.");
            }
        }
    }
}
