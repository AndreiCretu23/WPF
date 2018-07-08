using System;
using System.Windows;
using System.Windows.Input;

namespace Quantum.Command
{
    public delegate bool CommandCanExecute();
    public delegate void CommandExecute();

    public class ManagedCommand : CommandBase, IManagedCommand
    {
        public override bool CanExecute(object parameter) { return CanExecuteHandler(); }
        public override void Execute(object parameter) { ExecuteHandler(); }
        
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

        public MenuMetadataCollection MainMenuMetadata { get; set; } = new MenuMetadataCollection();
    }
}
