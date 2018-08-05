using System;

namespace Quantum.Metadata
{
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class PanelSelectionBinding : IDynamicPanelMetadata
    {
        public Type SelectionType { get; set; }

        public PanelSelectionBinding(Type selectionType)
        {
            SelectionType = selectionType;
        }
    }
}
