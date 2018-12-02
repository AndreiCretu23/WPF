using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Command
{
    /// <summary>
    /// Represents the basic abstract implementation of a multi static command. A multi static command 
    /// is a collection of static commands deduced from a settable getter delegate.
    /// </summary>
    /// <typeparam name="TCommand">The type of the commands associated with this MultiStaticCommand. This type parameter must be a type that implements IStaticCommand.</typeparam>
    public abstract class MultiStaticCommand<TCommand> : IMultiStaticCommand<TCommand>
        where TCommand : IStaticCommand
    {
        private IEnumerable<TCommand> subCommands;
        private Func<IEnumerable<TCommand>> commands = null;

        /// <summary>
        /// Returns the last computed command set.
        /// </summary>
        public IEnumerable<TCommand> SubCommands { get { if (subCommands == null) { ComputeCommands(); } return subCommands; } }
        
        /// <summary>
        /// A publicly settable delegate which computes the sub-static commands associated with this MultiStaticCommand.
        /// </summary>
        public Func<IEnumerable<TCommand>> Commands
        {
            private get { return commands; }
            set { subCommands = null; commands = value; }
        }

        /// <summary>
        /// Occurs when the set of commands have been computed from the command getter delegate. 
        /// The first parameter represents the old command set, and the second parameter represents the new comand set.
        /// </summary>
        public event Action<IEnumerable<TCommand>, IEnumerable<TCommand>> OnCommandsComputed;

        /// <summary>
        /// Computes the commands using the "Commands" delegate and notifies the listenes that the set of sub-static commands associated 
        /// with this MultiStaticCommand has been generated.
        /// </summary>
        public void ComputeCommands()
        {
            var newCommands = Commands?.Invoke() ?? Enumerable.Empty<TCommand>();
            var oldCommands = subCommands ?? Enumerable.Empty<TCommand>();

            subCommands = newCommands;
            OnCommandsComputed?.Invoke(oldCommands, newCommands);
        }

    }
}
