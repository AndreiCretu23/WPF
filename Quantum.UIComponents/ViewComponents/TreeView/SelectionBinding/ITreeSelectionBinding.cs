using Quantum.Common;
using Quantum.Services;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents a handler used to bind a tree view model's item selection states to a prism selection.
    /// </summary>
    public interface ITreeSelectionBinding : IDestructible
    {
        /// <summary>
        /// Gets the tree view model on which this selection binding is set.
        /// </summary>
        ITreeViewModel Owner { get; }
        

        /// <summary>
        /// Gets selection the tree view model items are bound to.
        /// </summary>
        ISelection BoundSelection { get; }


        /// <summary>
        /// Gets the currently selected tree view model item. <para/>
        /// IMPORTANT : If none or more than one item is selected, this property will be null;
        /// </summary>
        ITreeViewModelItem SelectedItem { get; }


        /// <summary>
        /// Gets the currently selected tree view model items in the context of this selection binding.
        /// </summary>
        IEnumerable<ITreeViewModelItem> SelectedItems { get; }
        
        
        /// <summary>
        /// Initializes this selection binding in the context of the specified tree view model.
        /// </summary>
        /// <param name="viewModel"></param>
        void Initialize(ITreeViewModel viewModel);


        /// <summary>
        /// Gets or sets a value indicating if, in the case of multiple selection, the entire content of the owner should be synced by value.
        /// If true, when an item is selected in the tree, all items having the same value will automatically be selected.
        /// </summary>
        bool SyncItems { get; set; }


        /// <summary>
        /// Handles the selection status of a newly created item and syncs it with the selected items collection.
        /// </summary>
        /// <param name="viewModelItem"></param>
        void OnItemCreated(ITreeViewModelItem viewModelItem);


        /// <summary>
        /// Handles the disposing of a tree view model item, removing it from the selected items collection if necessary.
        /// </summary>
        /// <param name="viewModelItem"></param>
        void OnItemDisposed(ITreeViewModelItem viewModelItem);


        /// <summary>
        /// Handles the updating of the bound selection when the selection state of a tree view model item changes.
        /// </summary>
        /// <param name="viewModelItem"></param>
        void OnItemSelectionStatusChanged(ITreeViewModelItem viewModelItem);
        
    }
}
