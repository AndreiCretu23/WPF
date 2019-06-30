using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    public interface ITreeViewModel : IViewModelOwner
    {
        /// <summary>
        /// Gets all items that are direct children of this tree view model.
        /// </summary>
        IEnumerable<ITreeViewModelItem> FirstLevelItems { get; }


        /// <summary>
        /// Gets all items currently owned by this tree view model, including 
        /// sub-items below the first layer.
        /// </summary>
        new IEnumerable<ITreeViewModelItem> Items { get; }
        

        /// <summary>
        /// Returns the currently selected tree view model item.
        /// If no item is selected or if this owner has a multiple selection, this value will be null.
        /// </summary>
        new ITreeViewModelItem SelectedItem { get; }


        /// <summary>
        /// Returns the currently selected tree view model items.
        /// </summary>
        new IEnumerable<ITreeViewModelItem> SelectedItems { get; }


        /// <summary>
        /// Notifies this tree view model that a new item has been created as part of it's logical descendants.
        /// </summary>
        /// <param name="item"></param>
        void NotifyItemCreated(ITreeViewModelItem item);


        /// <summary>
        /// Notifies this tree view model that an item has been removed from it's logical descendants.
        /// </summary>
        /// <param name="item"></param>
        void NotifyItemDisposed(ITreeViewModelItem item);


        /// <summary>
        /// Notifies this tree view model that the selection state of an item which is part of it's logical descendants has changed.
        /// </summary>
        /// <param name="item"></param>
        void NotifyItemSelectionChanged(ITreeViewModelItem item);


        // <summary>
        /// Gets the strategy type used to store this tree view model item's expansion states.
        /// </summary>
        TreeExpansionRetainingStrategy ExpansionRetainingStrategy { get; }


        /// <summary>
        /// Represents a context-dependant state retainer, used to cache-in item expansion values and 
        /// determine the expansion state of items after content invalidation operations.
        /// </summary>
        ITreeExpansionRetainer ExpansionRetainer { get; }


        /// <summary>
        /// Gets the currently active selection binding that is used to handle all items.
        /// </summary>
        ITreeSelectionBinding SelectionBinding { get; }

    }
}
