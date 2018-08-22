using System;
using System.Windows.Input;

namespace Quantum.Command
{
    public delegate bool DependencyCommandCanExecute<T>(T obj);
    public delegate void DependencyCommandExecute<T>(T obj);

    public abstract class DependencyCommand<T> : UICommand, IDependencyCommand
        where T : class
    {
        public override bool CanExecute(object parameter) { return CanExecuteHandler(parameter as T); }
        public override void Execute(object parameter) { ExecuteHandler(parameter as T); }

        public Type DependencyType { get { return typeof(T); } }

        private DependencyCommandCanExecute<T> canExecuteHandler;
        public DependencyCommandCanExecute<T> CanExecuteHandler
        {
            get
            {
                return canExecuteHandler ?? (o => true);
            }
            set
            {
                canExecuteHandler = value;
                RaiseCanExecuteChanged();
            }
        }

        private DependencyCommandExecute<T> executeHandler;
        public DependencyCommandExecute<T> ExecuteHandler
        {
            get
            {
                return executeHandler ?? (o => { });
            }
            set
            {
                executeHandler = value;
            }
        }

    }
}
