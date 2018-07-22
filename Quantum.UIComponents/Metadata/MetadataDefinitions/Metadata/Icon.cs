using System;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class Icon : IMenuMetadata, ISubMenuMetadata
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
    }
}
