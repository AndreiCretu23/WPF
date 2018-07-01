using System;

namespace Quantum.Command
{
    public class CheckChanged : IMainMenuMetadata
    {
        public bool SupportsMultiple { get { return false; } }
        public Action<bool> OnCheckChanged { get; private set; }
        public CheckChanged(Action<bool> onCheckChanged)
        {
            OnCheckChanged = onCheckChanged;
        }
    }
}
