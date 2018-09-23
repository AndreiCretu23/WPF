using System.Collections.Generic;
using System.Windows;

namespace Quantum.UIComponents
{
    /// <summary>
    /// This service is responsible for the management of all the dialogs inside the application. 
    /// It is be used to register, resolve, and show dialogs by their definition / view model types.
    /// </summary>
    public interface IDialogManagerService
    {
        /// <summary>
        /// Returns all the dialog definitions currently registered in the dialog manager.
        /// </summary>
        IEnumerable<IDialogDefinition> RegisteredDefinitions { get; }

        /// <summary>
        /// Registers the specified dialog definition in the dialog manager : 
        /// Registers the type of the view in the container and stores the view/viewModel types.
        /// </summary>
        /// <param name="definition"></param>
        void RegisterDialogDefinition(IDialogDefinition definition);

        /// <summary>
        /// Registers all the specified dialog definitions in the dialog manager.
        /// </summary>
        /// <param name="definitions"></param>
        void RegisterAllDefinitions(IEnumerable<IDialogDefinition> definitions);

        /// <summary>
        /// Resolves the specified view model type from the container and then searches 
        /// for the associated view type. Then, it instantiates the view, sets it's data context, 
        /// and shows the dialog.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the dialog's view model.</typeparam>
        /// <returns>The result of the dialog.</returns>
        bool? ShowDialog<TViewModel>() where TViewModel : IDialogViewModel;

        /// <summary>
        /// Searches for the associated view of the specified view model (by type).
        /// Then, it instatiates the view, sets it's data context and shows the dialog.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the dialog's view model.</typeparam>
        /// <param name="viewModel">The view model instance that is to be used as the dialog's data context.</param>
        /// <returns>The result of the dialog.</returns>
        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogViewModel;
        
        MessageBoxResult ShowMessageBox(string messageBoxText);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options);
    }
}
