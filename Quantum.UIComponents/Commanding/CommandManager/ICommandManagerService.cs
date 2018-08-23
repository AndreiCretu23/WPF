using System;
using System.Collections.Generic;

namespace Quantum.Command
{
    public interface ICommandManagerService
    {
        IEnumerable<object> Commands { get; }
        IEnumerable<IManagedCommand> ManagedCommands { get; }
        IEnumerable<IMultiManagedCommand> MultiManagedCommands { get; }

        void RegisterCommandContainer<IContainer, TContainer>() where TContainer : class, IContainer
                                                                where IContainer : class, ICommandContainer;

        object GetCommand<TCommandContainer>(Expression<Func<TCommandContainer, object>> commandProperty)
            where TCommandContainer : ICommandContainer;

        TCommand GetCommand<TCommandContainer, TCommand>(Expression<Func<TCommandContainer, TCommand>> commandProperty)
            where TCommandContainer : ICommandContainer;
    }
}
