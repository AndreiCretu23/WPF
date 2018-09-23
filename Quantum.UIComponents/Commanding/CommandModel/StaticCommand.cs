namespace Quantum.Command
{
    public delegate bool CommandCanExecute();
    public delegate void CommandExecute();

    /// <summary>
    /// Provides the basic abstract implementation of a parameterless UICommand.
    /// </summary>
    public abstract class StaticCommand : UICommand, IStaticCommand
    {
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// Returns the logical value returned by the (settable) CanExecuteHandler delegate.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns></returns>
        public override bool CanExecute(object parameter) { return CanExecuteHandler(); }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// Invokes the (settable) ExecuteHandler delegate.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object parameter) { ExecuteHandler(); }

        private CommandCanExecute canExecuteHandler;
        /// <summary>
        /// A settable delegate that determines whether the command can execute or not in it's current state. 
        /// If not set (left null), the command will be always validates.
        /// </summary>
        public CommandCanExecute CanExecuteHandler
        {
            get
            {
                return canExecuteHandler ?? (() => true);
            }
            set
            {
                canExecuteHandler = value;
                RaiseCanExecuteChanged();
            }
        }


        private CommandExecute executeHandler;
        /// <summary>
        /// Defines the delegate that is to be called when the command is invoked.
        /// </summary>
        public CommandExecute ExecuteHandler
        {
            get
            {
                return executeHandler ?? (() => { });
            }
            set
            {
                executeHandler = value;
            }
        }
    }
}
