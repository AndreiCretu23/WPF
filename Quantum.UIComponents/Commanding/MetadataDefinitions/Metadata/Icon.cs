using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    public class Icon : IMainMenuMetadata
    {
        public bool SupportsMultiple { get { return false; } }
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
