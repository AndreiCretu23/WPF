using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    public class ToolTip : IMainMenuMetadata
    {
        public ToolTip(string toolTip)
        {
            Value = toolTip;
        }

        public bool SupportsMultiple { get { return false; } }
        public string Value { get; private set; }
    }
}
