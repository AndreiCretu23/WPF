using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    [AttributeUsage(validOn:AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MandatoryAttribute : Attribute
    {
        public bool IsMandatory { get; private set; }
        
        public MandatoryAttribute(bool isMandatory)
        {
            IsMandatory = isMandatory;
        }
    }

    [AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SupportsMultipleAttribute : Attribute
    {
        public bool SupportsMultiple { get; private set; }

        public SupportsMultipleAttribute(bool supportsMultiple)
        {
            SupportsMultiple = supportsMultiple;
        }
    }
}
