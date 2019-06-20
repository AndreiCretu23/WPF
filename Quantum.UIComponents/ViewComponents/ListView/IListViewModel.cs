using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the basic contract for the data context model of a list component.
    /// </summary>
    public interface IListViewModel : IViewModelOwner
    {
        /// <summary>
        /// A collection of all the items of this list view model owner.
        /// </summary>
        new IEnumerable<IListViewModelItem> Items { get; }
        

        /// <summary>
        /// Returns the currently selected list view model item.
        /// If no item is selected or if this owner has a multiple selection, this value will be null.
        /// </summary>
        new IListViewModelItem SelectedItem { get; }
        

        /// <summary>
        /// Returns the currently selected list view model items.
        /// </summary>
        new IEnumerable<IListViewModelItem> SelectedItems { get; }
    }
}
