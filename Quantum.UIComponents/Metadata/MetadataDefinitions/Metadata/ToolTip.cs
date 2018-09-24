using Quantum.Common;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used to attach a ToolTip to it's UIDefinition owner.<para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class ToolTip : IAssertable, IMainMenuMetadata, ISubMainMenuMetadata, IPanelMenuEntryMetadata
    {
        public ToolTip(string toolTip)
        {
            Value = toolTip;
        }
        
        public string Value { get; private set; }

        [DebuggerHidden]
        public void Assert(string objName = null)
        {
            if(Value == null)
            {
                throw new Exception($"Error : {objName ?? String.Empty} contains a ToolTip metadata definition that has a null value.");
            }
        }
    }
}
