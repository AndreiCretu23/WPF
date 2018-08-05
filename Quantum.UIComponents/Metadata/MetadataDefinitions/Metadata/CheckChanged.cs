using Quantum.Common;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class CheckChanged : IAssertable, IMenuMetadata, ISubMenuMetadata
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
