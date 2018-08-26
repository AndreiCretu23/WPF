using System.Collections.Generic;
using System.Windows;

namespace Quantum.UIComponents
{
    public interface IDialogManagerService
    {
        IEnumerable<IDialogDefinition> RegisteredDefinitions { get; }

        void RegisterDialogDefinition(IDialogDefinition definition);
        void RegisterAllDefinitions(IEnumerable<IDialogDefinition> definitions);

        bool? ShowDialog<TViewModel>() where TViewModel : IDialogViewModel;
        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogViewModel;
        
        MessageBoxResult ShowMessageBox(string messageBoxText);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult);
        MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options);
    }
}
