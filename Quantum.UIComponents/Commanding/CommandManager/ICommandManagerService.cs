﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Quantum.Command
{
    /// <summary>
    /// This service is responsible for the registration / caching of all the commands in the application(defined in various command containers).
    /// It is used by various components of the framework in order to extract commands and process them depending on their type/metadata.
    /// </summary>
    public interface ICommandManagerService
    {
        /// <summary>
        /// Returns all commands registered in the command cache.
        /// </summary>
        IEnumerable<object> Commands { get; }

        /// <summary>
        /// Returns all commands of type IGlobalCommand registered in the command cache.
        /// </summary>
        IEnumerable<IGlobalCommand> GlobalCommands { get; }

        /// <summary>
        /// Returns all commands of type IMultiGlobalCommand registered in the command cache.
        /// </summary>
        IEnumerable<IMultiGlobalCommand> MultiGlobalCommands { get; }

        /// <summary>
        /// Registers all command properties in the specified command container in the command cache.
        /// Command properties decorated with the IgnoreCommand attribute will be skipped over.
        /// </summary>
        /// <typeparam name="IContainer"></typeparam>
        /// <typeparam name="TContainer"></typeparam>
        void RegisterCommandContainer<IContainer, TContainer>() where TContainer : class, IContainer
                                                                where IContainer : class, ICommandContainer;

        /// <summary>
        /// Looks up for the command (defined in the specified command container) represented by the given expression in the command cache and returns it.
        /// If the command is not found in the cache, an exception will be thrown.
        /// </summary>
        /// <typeparam name="TCommandContainer"></typeparam>
        /// <param name="commandProperty"></param>
        /// <returns></returns>
        object GetCommand<TCommandContainer>(Expression<Func<TCommandContainer, object>> commandProperty)
            where TCommandContainer : ICommandContainer;

        /// <summary>
        /// Loopks up for the command (defined in the specified command container) of the specified type represented by the given expression 
        /// in the command cache and returns it. If the command is not found, an exception will be thrown.
        /// </summary>
        /// <typeparam name="TCommandContainer"></typeparam>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="commandProperty"></param>
        /// <returns></returns>
        TCommand GetCommand<TCommandContainer, TCommand>(Expression<Func<TCommandContainer, TCommand>> commandProperty)
            where TCommandContainer : ICommandContainer;

        /// <summary>
        /// Returns the name of the specified command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string GetCommandName(object command);

        /// <summary>
        /// Returns a value indicating if the specified command in registered in the CommandManager.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        bool IsRegistered(object command);
    }
}
