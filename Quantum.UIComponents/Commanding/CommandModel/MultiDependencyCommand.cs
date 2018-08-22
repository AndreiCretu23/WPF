using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Command
{
    public abstract class MultiDependencyCommand<T, TCommand> : IMultiDependencyCommand<TCommand>
        where T : class
        where TCommand : DependencyCommand<T>
    {
        public Type DependencyType { get { return typeof(T); } }
        public event Action<IEnumerable<TCommand>> OnCommandsComputed;

        Func<T, IEnumerable<TCommand>> Commands { get; set; } = o => Enumerable.Empty<TCommand>();

        public IEnumerable<TCommand> ComputeCommands(object o)
        {
            var commands = Commands?.Invoke(o as T) ?? Enumerable.Empty<TCommand>();
            OnCommandsComputed?.Invoke(commands);
            return commands;
        }
    }
}
