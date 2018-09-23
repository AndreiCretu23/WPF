using System;
using System.Windows.Input;

namespace Quantum.Command
{
    public delegate bool DependencyCommandCanExecute<T>(T obj);
    public delegate void DependencyCommandExecute<T>(T obj);

    /// <summary>
    /// Provides the basic abstract implementation of a UICommand which takes a parameter of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of this dependency command</typeparam>
    public abstract class DependencyCommand<T> : UICommand, IDependencyCommand
    {
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// Returns the logical value returned by the (settable) CanExecuteHandler delegate, passing it the associated parameter. <para/>
        /// If the associated parameter is not convertible to the generic type of the dependency command, an InvalidCastException will be thrown.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns></returns>
        public override bool CanExecute(object parameter) { return CanExecuteHandler((T)parameter); }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// Invokes the (settable) ExecuteHandler delegate, passing it it's the associated parameter. <para/>
        /// If the associated parameter is not convertible to the generic type of the dependency command, an InvalidCastException will be thrown.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object parameter) { ExecuteHandler((T)parameter); }

        /// <summary>
        /// Returns the type of the parameter of this dependency command.
        /// </summary>
        public Type DependencyType { get { return typeof(T); } }


        private DependencyCommandCanExecute<T> canExecuteHandler;
        /// <summary>
        /// A settable delegate that determines whether the command can execute or not in it's current state, given the associated parameter. 
        /// If not set (left null), the command will be always validates.
        /// </summary>
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
        /// <summary>
        /// Defines the delegate that is to be called when the command is invoked, given the associated parameter.
        /// </summary>
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
