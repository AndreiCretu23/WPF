using Quantum.Common;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type is used to attach an icon to it's UIDefinition owner. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
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
