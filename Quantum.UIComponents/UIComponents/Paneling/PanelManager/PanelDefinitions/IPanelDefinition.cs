using System;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Provides the basic contract for a panel definition.
    /// </summary>
    public interface IPanelDefinition
    {
        /// <summary>
        /// Returns the type of the interface the view implements.
        /// </summary>
        Type IView { get; }

        /// <summary>
        /// Returns the type of the view associated to this PanelDefinition.
        /// </summary>
        Type View { get; }

        /// <summary>
        /// Returns the type of the interface the viewModel implements.
        /// </summary>
        Type IViewModel { get; }

        /// <summary>
        /// Returns the type of the viewModel associated to the view of this PanelDefinition.
        /// </summary>
        Type ViewModel { get; }
    }
}
