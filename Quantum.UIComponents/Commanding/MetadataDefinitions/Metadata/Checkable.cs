using System;

namespace Quantum.Command
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class Checkable : IMenuMetadata, ISubMenuMetadata
    {
        public bool Value { get; private set; }
        public Checkable(bool isCheckable) {
            Value = isCheckable;
        }
        public Checkable(Func<bool> isCheckableGetter) {
            Value = isCheckableGetter();
        }
    }
}
