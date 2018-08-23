using Quantum.Common;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class Icon : IAssertable, IMainMenuMetadata, ISubMainMenuMetadata, IPanelMenuEntryMetadata
    {
        public string IconPath { get; private set; }
        public Icon(string iconPath)
        {
            IconPath = iconPath;
        }
        public Icon(Func<string> iconPathGetter)
        {
            IconPath = iconPathGetter();
        }

        [DebuggerHidden]
        public void Assert(string objName = null)
        {
            if(IconPath == null)
            {
                throw new Exception($"Error : {objName ?? String.Empty} contains an Icon metadata definition that has a null value.");
            }
        }
    }
}
