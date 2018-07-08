using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class ToolTip : IMenuMetadata, ISubMenuMetadata
    {
        public ToolTip(string toolTip)
        {
            Value = toolTip;
        }
        
        public string Value { get; private set; }
    }
}
