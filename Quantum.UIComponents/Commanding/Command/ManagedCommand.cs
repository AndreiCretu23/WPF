using System;
using System.Windows;
using System.Windows.Input;

namespace Quantum.Command
{
    public delegate bool CommandCanExecute();
    public delegate void CommandExecute();

    public class ManagedCommand : IManagedCommand
    {
        private event EventHandler canExecuteChanged;
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                canExecuteChanged += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                canExecuteChanged -= value;
            }
        }
        
        public bool CanExecute(object parameter) { return CanExecuteHandler(); }
        public void Execute(object parameter) { ExecuteHandler(); }
        
        public void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                canExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        private CommandCanExecute canExecuteHandler;
        public CommandCanExecute CanExecuteHandler
        {
            get {
                return canExecuteHandler ?? (() => true);
            }
            set {
                canExecuteHandler = value;
                RaiseCanExecuteChanged();
            }
        }

        private CommandExecute executeHandler;
        public CommandExecute ExecuteHandler
        {
            get {
                return executeHandler ?? (() => { });
            }
            set {
                executeHandler = value;
            }
        }

        public CommandMetadataCollection CommandMetadata { get; set; } = new CommandMetadataCollection();
        public MainMenuMetadataCollection MainMenuMetadata { get; set; } = new MainMenuMetadataCollection();
    }
}
