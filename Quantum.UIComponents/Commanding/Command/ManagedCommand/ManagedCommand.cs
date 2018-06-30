using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quantum.Command
{
    public delegate bool CommandCanExecute(object parameter);
    public delegate void CommandExecute(object parameter);

    public class ManagedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        private CommandCanExecute canExecuteHandler;
        public CommandCanExecute CanExecuteHandler
        {
            get {
                return canExecuteHandler ?? (o => true);
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
                return executeHandler ?? (o => { });
            }
            set {
                executeHandler = value;
            }
        }

        public ManagedCommandMetadataCollection Metadata { get; set; } = new ManagedCommandMetadataCollection();
    }
}
