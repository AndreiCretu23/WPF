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
        private IEnumerable<TCommand> subCommands;
        private Func<T, IEnumerable<TCommand>> commands = null;
        

        /// <summary>
        /// Returns the dependency type of the command type associated with this MultiDependencyCommand.
        /// </summary>
        public Type DependencyType { get { return typeof(T); } }
        
        
        /// <summary>
        /// Returns the last computed command set.
        /// </summary>
        public IEnumerable<TCommand> SubCommands { get { return null; } }


        /// <summary>
        /// A publicly settable delegate which computes the sub-dependency commands associated with this MultiDependencyCommand.
        /// </summary>
        public Func<T, IEnumerable<TCommand>> Commands
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
        /// Computes the commands using the "Commands" delegate and notifies the listenes that the set of sub-dependency commands associated 
        /// with this MultiDependencyCommand has been generated.
        /// </summary>
        public void ComputeCommands(object o)
        {
            var newCommands = Commands?.Invoke((T)o) ?? Enumerable.Empty<TCommand>();
            var oldCommands = subCommands ?? Enumerable.Empty<TCommand>();

            if(newCommands.Any(cmd => cmd.DependencyType != DependencyType)) {
                throw new Exception($"Error : The dependency type of all the dependency commands returned by the getter delegate must match the dependency type of the MultiDependencyCommand.");
            }

            subCommands = newCommands;
            OnCommandsComputed?.Invoke(oldCommands, newCommands);
        }


    }
}
