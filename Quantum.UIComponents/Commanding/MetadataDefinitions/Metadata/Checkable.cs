using System;

namespace Quantum.Command
{
    public class Checkable : IMainMenuMetadata
    {
        public bool SupportsMultiple { get { return false; } }
        public bool Value { get; private set; }
        public Checkable(bool isCheckable) {
            Value = isCheckable;
        }
        public Checkable(Func<bool> isCheckableGetter) {
            Value = isCheckableGetter();
        }
    }
}
