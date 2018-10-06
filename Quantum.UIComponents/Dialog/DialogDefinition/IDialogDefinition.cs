using System;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Provides the basic contract for any dialog definition.
    /// </summary>
    public interface IDialogDefinition
    {
        /// <summary>
        /// Returns the interface type the view implements. This type must extend IDialogWindow.
        /// </summary>
        Type IView { get; }

        /// <summary>
        /// Returns the type of the view of the dialog. This type must extend DialogWindow.
        /// </summary>
        Type View { get; }

        /// <summary>
        /// Returns the interface type the view model implements. This type must extend IDialogViewModel.
        /// </summary>
        Type IViewModel { get; }

        /// <summary>
        /// Returns the type of the view model. This type must extend DialogViewModel.
        /// </summary>
        Type ViewModel { get; }
        
    }
}
