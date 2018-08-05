using Quantum.Common;
using Quantum.Services;
using Quantum.Utils;
using System;

namespace Quantum.Metadata
{
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class PanelSelectionBinding : IAssertable, IDynamicPanelMetadata
    {
        public Type SelectionType { get; set; }

        public PanelSelectionBinding(Type selectionType)
        {
            SelectionType = selectionType;
        }

        public void Assert(string objName = null)
        {
            if(SelectionType == null)
            {
                throw new Exception($"Error : {objName ?? String.Empty} contains a PanelSelectionBinding Metadata Definition that has a null SelectionType.");
            }

            if(!SelectionType.IsSubclassOfRawGeneric(typeof(MultipleSelection<>)))
            {
                throw new Exception($"Error : {objName ?? String.Empty} contains a PanelSelectionBinding Metadata Definition that does not have a valid SelectionType." +
                    $"The SelectionType must Extend MultipleSelection<T>. It's purpose is to bind the viewModels of the active panels to a collection.");
            }
        }
    }
}
