using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quantum.Command
{
    public class ManagedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged; 
        public bool CanExecute(object parameter) { return CanExecuteHandler(parameter); }
        public void Execute(object parameter) { ExecuteHandler(parameter); }

        public delegate bool CommandPredicate(object obj);
        public delegate void CommandHandler(object obj);

        private CommandPredicate canExecuteHandler;
        public CommandPredicate CanExecuteHandler
        {
            get {
                if(canExecuteHandler == null) {
                    return obj => true;
                }
                return canExecuteHandler;
            }
            set {
                canExecuteHandler = value;
            }
        }

        private CommandHandler executeHandler;
        public CommandHandler ExecuteHandler
        {
            get {
                if(executeHandler == null) {
                    return obj => { };
                }
                return executeHandler;
            }
            set {
                executeHandler = value;
            }
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public MetadataCollection Metadata { get; set; } = new MetadataCollection();
    }
}
