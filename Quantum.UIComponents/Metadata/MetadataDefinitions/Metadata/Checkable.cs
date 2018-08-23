using System;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    public class Checkable : IMainMenuMetadata, ISubMainMenuMetadata
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
