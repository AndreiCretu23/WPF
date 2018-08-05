using Quantum.Common;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class ToolTip : IAssertable, IMenuMetadata, ISubMenuMetadata
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
