using Quantum.Common;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Diagnostics;

namespace Quantum.Metadata
{
    /// <summary>
    /// This metadata type hints the owner that an invalidation is required when the selection of the specified type 
    /// resolved from the the event aggregator instance of the application's container changes. 
    /// The invalidation effect varies depending on the owner of this metadata type. <para/>
    /// (HINT : Metadata types do different things depending on the parent collection that contains them. 
    /// Clarifications regarding what a particular metadata type does can be found in the summaries of metadacollections / components that can contain it).
    /// </summary>
    [Mandatory(false)]
    [SupportsMultiple(true)]
    public class AutoInvalidateOnSelection : IAssertable, ICommandMetadata, IMultiCommandMetadata, ISubCommandMetadata, IToolBarMetadata, IStaticPanelMetadata, IDynamicPanelMetadata
    {
        public Type SelectionType { get; private set; }
        public AutoInvalidateOnSelection(Type selectionType)
        {
            SelectionType = selectionType;
        }

        [DebuggerHidden]
        public void Assert(string objName = null)
        {
            if (SelectionType == null)
            {
                throw new Exception($"Error : {objName ?? String.Empty} contains an AutoInvalidateOnSelection metadata definition that has a null event type.");
            }

            if (!SelectionType.IsSubclassOfRawGeneric(typeof(SelectionBase<>)))
            {
                throw new Exception($"Error : {objName ?? String.Empty} contains an AutoInvalidateOnSelection metadata definition that does not have a valid selection type. " +
                    $"The type of the Selection must either extend SingleSelection<TSelection> or MultipleSelection<TSelection>.");
            }
        }
    }
}
