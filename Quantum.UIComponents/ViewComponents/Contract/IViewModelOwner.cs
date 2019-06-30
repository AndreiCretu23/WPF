using Quantum.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the basic contract for any view model that has sub-items.
    /// </summary>
    public interface IViewModelOwner : IViewModel
    {
        /// <summary>
        /// Gets a value indicating whether or not multiple selection is allowed for the items of this view model owner.
        /// </summary>
        bool AllowMultipleSelection { get; }


        /// <summary>
        /// A collection of all the items of this view model owner.
        /// </summary>
        IEnumerable<IViewModelItem> Items { get; }


        /// <summary>
        /// The values of the view model items owned by this view model owner.
        /// </summary>
        IEnumerable<object> Values { get; }


        /// <summary>
        /// Returns the currently selected view model item.
        /// If no item is selected or if this owner has a multiple selection, this value will be null.
        /// </summary>
        IViewModelItem SelectedItem { get; }


        /// <summary>
        /// Returns the value of the currently slected view model item.
        /// If no item is selected or if this owner has a multiple selection, this value will be null.
        /// </summary>
        object SelectedValue { get; }


        /// <summary>
        /// Returns the currently selected view model items.
        /// </summary>
        IEnumerable<IViewModelItem> SelectedItems { get; }


        /// <summary>
        /// Returns the values of the currently selected view model items.
        /// </summary>
        IEnumerable<object> SelectedValues { get; }


        /// <summary>
        /// Invalidates the children of this view model item.
        /// </summary>
        void InvalidateChildren();
        

        /// <summary>
        /// Returns the icon manager used by this view model item owner to determine the icon values for it's children.
        /// </summary>
        IIconManagerService IconManager { get; }
    }
}
