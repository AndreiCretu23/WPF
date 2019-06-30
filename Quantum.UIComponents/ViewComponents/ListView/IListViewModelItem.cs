using Quantum.Common;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the basic contract for the data context model of a ListItem.
    /// </summary>
    public interface IListViewModelItem : IViewModelItem, IDestructible
    {
        /// <summary>
        /// The explicit list view model who owns this list view model item instance.
        /// </summary>
        new IListViewModel Owner { get; }


        /// <summary>
        /// Returns a value indicating if this list view model item has been initialized in the context of an owner.
        /// </summary>
        bool IsInitialized { get; }


        /// <summary>
        /// Initializes this list view model item instance in the context of the specified owner.
        /// </summary>
        /// <param name="owner"></param>
        void Initialize(IListViewModel owner);
    }
}
