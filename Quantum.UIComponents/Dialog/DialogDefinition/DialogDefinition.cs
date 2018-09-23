using System;

namespace Quantum.UIComponents
{
    /// <summary>
    /// Represents the definition of a dialog view that is to be registered in the dialog manager.
    /// </summary>
    /// <typeparam name="ITView">The interface type the dialog's view implements. This type must extend IDialogWindow.</typeparam>
    /// <typeparam name="TView">The type of the view of the dialog. This type must extend DialogWindow.</typeparam>
    /// <typeparam name="ITViewModel">The type of the interface the view model of the dialog implements. This type must extend IDialogViewModel.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model of the dialog. This type must extend DialogViewModel.</typeparam>
    public class DialogDefinition<ITView, TView, ITViewModel, TViewModel> : IDialogDefinition
        where ITView : IDialogWindow
        where TView : DialogWindow, ITView, new()
        where ITViewModel : IDialogViewModel
        where TViewModel : DialogViewModel, ITViewModel
    {
        /// <summary>
        /// Returns the type of the view of the dialog. This type must extend DialogWindow.
        /// </summary>
        public Type View => typeof(TView);

        /// <summary>
        /// Returns the type of the view model. This type must extend DialogViewModel.
        /// </summary>
        public Type ViewModel => typeof(TViewModel);

        /// <summary>
        /// Returns the interface type the view implements. This type must extend IDialogWindow.
        /// </summary>
        public Type IView => typeof(ITView);

        /// <summary>
        /// Returns the interface type the view model implements. This type must extend IDialogViewModel.
        /// </summary>
        public Type IViewModel => typeof(ITViewModel);

        /// <summary>
        /// Returns a value indicating if the view model type should be registered as a single instance in the 
        /// container by the dialog manager. If true, each time the dialog will be shown, or an instance of 
        /// the associated view model type will be requested from the container, the same instance will be 
        /// returned. Otherwise, a new instance will be created every time the dialog is shown / a view model 
        /// instance is requested from the container.
        /// </summary>
        public bool SingleViewModelInstance { get; }

        /// <summary>
        /// Creates a new instance of the DialogDefinition class.
        /// </summary>
        /// <param name="singleViewModelInstance"> A flag indicating if the view model type should be registered 
        /// as a single instance in the container by the dialog manager. If true, each time the dialog will 
        /// be shown, or an instance of the associated view model type will be requested from the container, 
        /// the same instance will be returned. Otherwise, a new instance will be created every time the dialog 
        /// is shown / a view model instance is requested from the container.</param>
        public DialogDefinition(bool singleViewModelInstance = false)
        {
            SingleViewModelInstance = singleViewModelInstance;
        }

    }
}
