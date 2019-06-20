using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the baseic contract for any ViewModel that is a wrapper over an item.
    /// </summary>
    public interface IViewModelItem : IViewModel
    {
        /// <summary>
        /// Gets value of the view model item.
        /// </summary>
        object Value { get; }
        

        /// <summary>
        /// Returns the implementation-independant type of the value of this view model item.
        /// </summary>
        Type ValueType { get; }


        /// <summary>
        /// Gets the ancestor who owns this view model item.
        /// </summary>
        IViewModelOwner Owner { get; }
        

        /// <summary>
        /// Gets publicly displayed header of this view model item.
        /// </summary>
        string Header { get; }


        /// <summary>
        /// Gets the effective icon of this view model item.
        /// </summary>
        string Icon { get; }
        

        /// <summary>
        /// Gets or sets a value indicating if this view model item is selected in the context of it's owner.
        /// </summary>
        bool IsSelected { get; set; }

        
    }
}
