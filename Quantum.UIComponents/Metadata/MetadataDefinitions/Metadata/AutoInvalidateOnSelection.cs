using System;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class AutoInvalidateOnSelection : ICommandMetadata, IMultiMenuMetadata, IToolBarMetadata
    {
        public Type SelectionType { get; private set; }
        public AutoInvalidateOnSelection(Type selectionType)
        {
            SelectionType = selectionType;
        }
    }
}
