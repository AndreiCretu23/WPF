using Quantum.Services;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents a contract for default view model item selection types.
    /// If the selection binding of a view model owner is of this type, it means a selection binding has not been set explicitly.
    /// </summary>
    internal interface IDefaultViewModelItemSelection
    {
    }

    /// <summary>
    /// Represents a single selection that is not registered in the container.
    /// View model owners use custom-created instances of this class for selection-binding compatibility is case no selection binding is provided.
    /// </summary>
    internal class SingleViewModelItemSelection : SingleSelection<object>, IDefaultViewModelItemSelection
    {
    }
    
    /// <summary>
    /// Represents a multiple selection that is not registered in the container.
    /// View model owners use custom-created instances of this class for selection-binding compatibility is case no selection binding is provided.
    /// </summary>
    internal class MultipleViewModelItemSelection : MultipleSelection<object>, IDefaultViewModelItemSelection
    {
    }
}
