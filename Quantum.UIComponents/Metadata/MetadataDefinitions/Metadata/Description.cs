using Quantum.Common;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class Description : IAssertable, IMenuMetadata, ISubMenuMetadata, IPanelMenuEntryMetadata
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
