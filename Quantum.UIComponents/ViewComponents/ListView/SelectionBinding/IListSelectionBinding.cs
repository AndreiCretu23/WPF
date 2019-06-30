using Quantum.Common;
using Quantum.Services;
using System.Collections.Generic;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents a handler used to bind a list view model's item selection states to a prism selection.
    /// </summary>
    internal interface IListSelectionBinding : IDestructible
    {
        /// <summary>
        /// Gets the list view model on which this selection binding is set.
        /// </summary>
        IListViewModel Owner { get; }


        /// <summary>
        /// Gets the selection the list view model items are bound to.
        /// </summary>
        ISelection BoundSelection { get; }


        /// <summary>
        /// Gets the currently selected list view model item. <para/>
        /// IMPORTANT : If none or more than one item is selected, this property will be null.
        /// </summary>
        IListViewModelItem SelectedItem { get; }


        /// <summary>
        /// Gets the currently selected list view model items in the context of this selection binding.
        /// </summary>
        IEnumerable<IListViewModelItem> SelectedItems { get; }
        

        /// <summary>
        /// Gets or sets a value indicating if, in the case of multiple selection, the entire content of the owner should be synced by value.
        /// If true, when an item is selected in the list, all items having the same value will automatically be selected.
        /// </summary>
        bool SyncItems { get; set; }


        /// <summary>
        /// Initializes this selection binding in the context of the specified list view model owner.
        /// </summary>
        /// <param name="viewModel"></param>
        void Initialize(IListViewModel viewModel);


        /// <summary>
        /// Handles the selection status of a newly created item and syncs it with the selected items collection.
        /// </summary>
        /// <param name="viewModelItem"></param>
        void OnItemCreated(IListViewModelItem viewModelItem);


        /// <summary>
        /// Handles the disposing of a tree list model item, removing it from the selected items collection if necessary.
        /// </summary>
        /// <param name="viewModelItem"></param>
        void OnItemDisposed(IListViewModelItem viewModelItem);


        /// <summary>
        /// Handles the updating of the bound selection when the selection state of a list view model item changes.
        /// </summary>
        /// <param name="viewModelItem"></param>
        void OnItemSelectionStatusChanged(IListViewModelItem viewModelItem);
    }
}
