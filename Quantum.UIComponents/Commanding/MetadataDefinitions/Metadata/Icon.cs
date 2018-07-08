using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
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
