using System;
using System.Windows;
using System.Windows.Input;

namespace Quantum.Command
{
    public abstract class UICommand : IUICommand
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

        public void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                canExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
    }
}
