using System;

namespace Quantum.Command
{
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class AutoInvalidateOnSelection : ICommandMetadata, IMultiMenuMetadata
    {
        public Type SelectionType { get; private set; }
        public AutoInvalidateOnSelection(Type selectionType)
        {
            SelectionType = selectionType;
        }
    }
}
