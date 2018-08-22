namespace Quantum.Command
{
    public delegate bool CommandCanExecute();
    public delegate void CommandExecute();

    public abstract class StaticCommand : UICommand, IStaticCommand
    {
        public override bool CanExecute(object parameter) { return CanExecuteHandler(); }
        public override void Execute(object parameter) { ExecuteHandler(); }

        private CommandCanExecute canExecuteHandler;
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
