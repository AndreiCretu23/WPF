using System;

namespace Quantum.Command
{
    public class AutoInvalidateOnSelection : ICommandMetadata
    {
        public bool SupportsMultiple { get { return true; } }
        public Type SelectionType { get; private set; }
        public AutoInvalidateOnSelection(Type selectionType)
        {
            SelectionType = selectionType;
        }
    }
}
