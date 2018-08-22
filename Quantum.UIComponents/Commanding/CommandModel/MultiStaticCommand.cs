using System;
using System.Collections.Generic;
using System.Linq;

namespace Quantum.Command
{
    public abstract class MultiStaticCommand<TCommand> : IMultiStaticCommand<TCommand>
        where TCommand : StaticCommand
    {
        public event Action<IEnumerable<TCommand>> OnCommandsComputed;

        public Func<IEnumerable<TCommand>> Commands { get; set; } = () => Enumerable.Empty<TCommand>();

        public IEnumerable<TCommand> ComputeCommands()
        {
            var commands = Commands?.Invoke() ?? Enumerable.Empty<TCommand>();
            OnCommandsComputed?.Invoke(commands);
            return commands;
        }
    }
}
