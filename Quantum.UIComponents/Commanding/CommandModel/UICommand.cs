using System;
using System.Windows;
using System.Windows.Input;

namespace Quantum.Command
{
    /// <summary>
    /// UI commands represent the most basic abstract implementation of a command.
    /// </summary>
    public abstract class UICommand : IUICommand
    {
        private event EventHandler canExecuteChanged;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
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

        /// <summary>
        /// Notifies the listeners that the logical condition of CanExecute has changed and needs to be re-evaluated.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                canExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }
        
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// Returns true if this command can be executed, false otherwise.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns></returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public abstract void Execute(object parameter);
    }
}
