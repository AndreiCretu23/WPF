using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Command
{
    /// <summary>
    /// Represents the basic abstract implementation of a multi dependency command. A multi dependency command 
    /// is a collection of dependency commands deduced from a settable getter delegate.
    /// </summary>
    /// <typeparam name="T">The type of dependency command parameter associated with the dependency command type asoociated with this MultiDependencyCommand.</typeparam>
    /// <typeparam name="TCommand">The type of the commands associated with this MultiDependencyCommand. This type parameter must be a type that implements IDependencyCommand.</typeparam>
    public abstract class MultiDependencyCommand<T, TCommand> : IMultiDependencyCommand<TCommand>
        where TCommand : IDependencyCommand
    {
        /// <summary>
        /// Returns the dependency type of the command type associated with this MultiDependencyCommand.
        /// </summary>
        public Type DependencyType { get { return typeof(T); } }
        

        /// <summary>
        /// Occurs when the set of commands have been computed from the command getter delegate.
        /// </summary>
        public event Action<IEnumerable<TCommand>> OnCommandsComputed;

        /// <summary>
        /// A publicly settable delegate which computes the sub-dependency commands associated with this MultiDependencyCommand.
        /// </summary>
        public Func<T, IEnumerable<TCommand>> Commands { private get; set; } = o => Enumerable.Empty<TCommand>();

        /// <summary>
        /// Computes the commands using the "Commands" delegate, notifies the listenes that the set of sub-dependency commands associated 
        /// with this MultiDependencyCommand has been generated and then returns the resulted commands.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TCommand> ComputeCommands(object o)
        {
            var commands = Commands?.Invoke((T)o) ?? Enumerable.Empty<TCommand>();
            if(commands.Any(cmd => cmd.DependencyType != DependencyType))
            {
                throw new Exception($"Error : The dependency type of all the dependency commands returned by the getter delegate must match the dependency type of the MultiDependencyCommand.");
            }
            OnCommandsComputed?.Invoke(commands);
            return commands;
        }
    }
}
