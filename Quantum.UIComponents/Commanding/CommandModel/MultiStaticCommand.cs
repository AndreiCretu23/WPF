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
        /// <summary>
        /// Occurs when the set of commands have been computed from the command getter delegate.
        /// </summary>
        public event Action<IEnumerable<TCommand>> OnCommandsComputed;

        /// <summary>
        /// A publicly settable delegate which computes the sub-static commands associated with this MultiStaticCommand.
        /// </summary>
        public Func<IEnumerable<TCommand>> Commands { private get; set; } = () => Enumerable.Empty<TCommand>();

        /// <summary>
        /// Computes the commands using the "Commands" delegate, notifies the listenes that the set of sub-static commands associated 
        /// with this MultiStaticCommand has been generated and then returns the resulted commands.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TCommand> ComputeCommands()
        {
            var commands = Commands?.Invoke() ?? Enumerable.Empty<TCommand>();
            OnCommandsComputed?.Invoke(commands);
            return commands;
        }
    }
}
