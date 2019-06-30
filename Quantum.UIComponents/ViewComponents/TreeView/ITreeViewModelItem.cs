using Quantum.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    public interface ITreeViewModelItem : IViewModelItem, IDestructible
    {
        
        /// <summary>
        /// Gets the type-explicit tree view model who owns this tree view model item instance.
        /// </summary>
        new ITreeViewModel Owner { get; }


        /// <summary>
        /// Gets the parent of this tree view model item instance.
        /// If this tree view model item instance is a direct child of it's owner, this property will be null.
        /// </summary>
        ITreeViewModelItem Parent { get; }


        /// <summary>
        /// Gets all the currently active direct children of this tree view model item.
        /// </summary>
        IEnumerable<ITreeViewModelItem> FirstLevelItems { get; }


        // <summary>
        /// Gets a value indicating if the children of this tree view model item have been initialized.
        /// The children have lazy loading, therefore, they will not be created until called upon : the first time the Children property getter is called.
        /// </summary>
        bool AreChildrenInitialized { get; }


        /// <summary>
        /// Gets or sets a value indicating if this tree view model item instance has been initialized in the context of an owner and a parent.
        /// </summary>
        bool IsInitialized { get; }


        /// <summary>
        /// Gets or sets a value indicating whether this tree view model item is expanded or not.
        /// </summary>
        bool IsExpanded { get; set; }


        /// <summary>
        /// Creates a path representation of this tree view model item using the headers of all the items up to the root.
        /// </summary>
        /// <param name="separator">The separator to use when joining the components of the resulted path.</param>
        /// <returns></returns>
        string CreatePath(string separator = "~");


        /// <summary>
        /// Returns a collection containined all ancestors of this tree view model item that satisfy the specified condition (if given).
        /// </summary>
        /// <param name="condition">The (optional) condition that a tree view model item must satisfy in order to be included in the result.</param>
        /// <returns></returns>
        IEnumerable<ITreeViewModelItem> GetAncestors(Predicate<ITreeViewModelItem> condition = null);


        /// <summary>
        /// Returns a collection containing this instance and all ancestors of this tree view model item that satisfy the specified condition (if given).
        /// </summary>
        /// <param name="condition">The (optional) condition that a tree view model item must satisfy in order to be included in the result.</param>
        /// <returns></returns>
        IEnumerable<ITreeViewModelItem> GetThisAndAncestors(Predicate<ITreeViewModelItem> condition = null);


        /// <summary>
        /// Returns a collection containing all currently active descendants of this tree view model item that satisfy the specified condition (if given).
        /// </summary>
        /// <param name="condition">The (optional) condition that a tree view model item must satisfy in order to be included in the result.</param>
        /// <returns></returns>
        IEnumerable<ITreeViewModelItem> GetDescendants(Predicate<ITreeViewModelItem> condition = null);


        /// <summary>
        /// Returns a collection containing this instance and all currently active descendants of this tree view model item that satisfy the specified condition (if given).
        /// </summary>
        /// <param name="condition">The (optional) condition that a tree view model item must satisfy in order to be included in the result.</param>
        /// <returns></returns>
        IEnumerable<ITreeViewModelItem> GetThisAndDescendants(Predicate<ITreeViewModelItem> condition);


        /// <summary>
        /// Initializes this tree view model item instance in the context of the specified owner and parent.
        /// </summary>
        /// <param name="owner">The owner of this tree view model item.</param>
        /// <param name="parent">The tree view model item parent of this instance, or null if this tree view model item is a direct child of it's owner.</param>
        void Initialize(ITreeViewModel owner, ITreeViewModelItem parent);


        /// <summary>
        /// Invalidates the sub-items of this tree view model item, updating the UI.
        /// </summary>
        void InvalidateChildren();
    }
}
