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

        /// <summary>
        /// Returns a value indicating if the view model type should be registered as a single instance in the 
        /// container by the dialog manager. If true, each time the dialog will be shown, or an instance of 
        /// the associated view model type will be requested from the container, the same instance will be 
        /// returned. Otherwise, a new instance will be created every time the dialog is shown / a view model 
        /// instance is requested from the container.
        /// </summary>
        bool SingleViewModelInstance { get; }
    }
}
