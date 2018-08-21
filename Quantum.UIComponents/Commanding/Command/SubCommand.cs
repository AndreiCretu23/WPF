using Quantum.Metadata;
using System.Collections;
using System.Collections.Generic;

namespace Quantum.Command
{
    public class SubCommand : CommandBase, ISubCommand
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

        public CommandMetadataCollection Metadata { get; set; } = new CommandMetadataCollection();
        public SubMenuMetadataCollection SubCommandMetadata { get; set; } = new SubMenuMetadataCollection();
    }

    public class SubCommandCollection : IEnumerable<SubCommand>
    {
        private List<SubCommand> internalCollection { get; set; } = new List<SubCommand>();

        public IEnumerator<SubCommand> GetEnumerator()
        {
            return internalCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalCollection.GetEnumerator();
        }

        public void Add(SubCommand subCommand)
        {
            internalCollection.Add(subCommand);
        }
    }
}
