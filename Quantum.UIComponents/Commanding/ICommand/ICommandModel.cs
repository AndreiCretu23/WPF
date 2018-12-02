using Quantum.Metadata;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Quantum.Command
{
    /// <summary>
    /// Represents the basic contract of a UICommand. <para/>
    /// UI commands represent the most basic abstract implementation of a command.
    /// </summary>
    public interface IUICommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Represents the basic contract of a StaticCommand. <para/>
    /// Static commands provide the basic abstract implementation of a parameterless UICommand.
    /// </summary>
    public interface IStaticCommand : IUICommand
    {
    }
    
    /// <summary>
    /// Represents the basic contract of a StaticMetadataOwnerCommand. <para/>
    /// Static metadata owner commands provide the basic abstract implementation of a static command that owns a MetadaCollection. Metadata collections 
    /// are a set of configurable data processed by various components of the framework when the command is defined in a 
    /// command container registered in the command manager service of the framework.
    /// </summary>
    /// <typeparam name="TMetadataCollection">The type of the metadata collection this static command owns.</typeparam>
    /// <typeparam name="TMetadataDefinition">The metadata filter type of the metadata collection owned by this static command.</typeparam>
    public interface IStaticMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition> : IStaticCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>
        where TMetadataDefinition : IMetadataDefinition
    {
        /// <summary>
        /// Represents a set of configurable metadata definitions processed by various components of the framework if the parent static command
        /// is defined in a command container registered in the command manager of the framework.
        /// </summary>
        TMetadataCollection Metadata { get; }
    }

    /// <summary>
    /// Represents the basic contract of a DependencyCommand. <para/>
    /// Dependency commands provide the basic abstract implementation of a UICommand which takes a parameter of a specified type.
    /// </summary>
    public interface IDependencyCommand : IUICommand
    {
        /// <summary>
        /// Returns the type of the parameter of this dependency command.
        /// </summary>
        Type DependencyType { get; }
    }
    
    /// <summary>
    /// Represents the basic contract of a DependencyMetadataOwnerCommand. <para/>
    /// Dependency metadata owner commands the basic abstract implementation of a dependency command that owns a MetadaCollection. Metadata collections 
    /// are a set of configurable data processed by various components of the framework when the command is defined in a 
    /// command container registered in the command manager service of the framework.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of this dependency command</typeparam>
    /// <typeparam name="TMetadataCollection">The type of the metadata collection this dependency command owns.</typeparam>
    /// <typeparam name="TMetadataDefinition">The metadata filter type of the metadata collection owned by this dependency command.</typeparam>
    public interface IDependencyMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition> : IDependencyCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>
        where TMetadataDefinition : IMetadataDefinition
    {
        /// <summary>
        /// Represents a set of configurable metadata definitions processed by various components of the framework if the parent dependency command
        /// is defined in a command container registered in the command manager of the framework.
        /// </summary>
        TMetadataCollection Metadata { get; }
    }

    /// <summary>
    /// Represents the basic contract of a MultiCommand. <para/>
    /// </summary>
    public interface IMultiCommand
    {
    }

    /// <summary>
    /// Represents the basic contract of a MultiCommand. <para/>
    /// </summary>
    public interface IMultiCommand<TCommand> : IMultiCommand
    {
    }
    

    /// <summary>
    /// Represents the basic contract of a MultiStaticCommand. <para/>
    /// Multi static commands provide the basic abstract implementation of a collection of static commands deduced from a settable getter delegate.
    /// </summary>
    /// <typeparam name="TCommand">The type of the commands associated with this MultiStaticCommand. This type parameter must be a type that implements IStaticCommand.</typeparam>
    public interface IMultiStaticCommand<TCommand> : IMultiCommand<TCommand>
        where TCommand : IStaticCommand
    {
        /// <summary>
        /// Returns the last computed command set.
        /// </summary>
        IEnumerable<TCommand> SubCommands { get; }

        /// <summary>
        /// Occurs when the set of commands have been computed from the command getter delegate. 
        /// The first parameter represents the old command set, and the second parameter represents the new comand set.
        /// </summary>
        event Action<IEnumerable<TCommand>, IEnumerable<TCommand>> OnCommandsComputed;

        /// <summary>
        /// Computes the commands using the "Commands" delegate and notifies the listenes that the set of sub-static commands associated 
        /// with this MultiStaticCommand has been generated.
        /// </summary>
        void ComputeCommands();
    }

    /// <summary>
    /// Represents the basic contract of a MultiStaticMetadataOwnerCommand. <para/>
    /// Multi static metadata owner commands provide the basic abstract implementation of a multi static command that owns a MetadaCollection. 
    /// Metadata collections are a set of configurable data processed by various components of the framework when the command is defined in a 
    /// command container registered in the command manager service of the framework.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command associated with this MultiStaticMetadataOwner command.</typeparam>
    /// <typeparam name="TMetadataCollection">The type of the metadata collection this multi static command owns.</typeparam>
    /// <typeparam name="TMetadataDefinition">The metadata filter type of the metadata collection owned by this multi static command.</typeparam>
    public interface IMultiStaticMetadataOwnerCommand<TCommand, TMetadataCollection, TMetadataDefinition> : IMultiStaticCommand<TCommand>
        where TCommand : IStaticCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        /// <summary>
        /// Represents a set of configurable metadata definitions processed by various components of the framework if the parent multi static command
        /// is defined in a command container registered in the command manager of the framework.
        /// </summary>
        TMetadataCollection Metadata { get; }
    }
    
    /// <summary>
    /// Represents the basic contract of a MultiDependencyCommand. <para/>
    /// Multi dependency commands provide the basic abstract implementation of a collection of dependency commands deduced from a settable getter delegate.
    /// </summary>
    /// <typeparam name="TCommand">The type of the commands associated with this MultiDependencyCommand. This type parameter must be a type that implements IDependencyCommand.</typeparam>
    public interface IMultiDependencyCommand<TCommand> : IMultiCommand<TCommand>
        where TCommand : IDependencyCommand
    {
        /// <summary>
        /// Returns the last computed command set.
        /// </summary>
        IEnumerable<TCommand> SubCommands { get; }

        /// <summary>
        /// Occurs when the set of commands have been computed from the command getter delegate. 
        /// The first parameter represents the old command set, and the second parameter represents the new comand set.
        /// </summary>
        event Action<IEnumerable<TCommand>, IEnumerable<TCommand>> OnCommandsComputed;

        /// <summary>
        /// Computes the commands using the "Commands" delegate and notifies the listenes that the set of sub-dependency commands associated 
        /// with this MultiDependencyCommand has been generated.
        /// </summary>
        void ComputeCommands(object o);

        /// <summary>
        /// Returns the dependency type of the command type associated with this MultiDependencyCommand.
        /// </summary>
        Type DependencyType { get; }
    }




    /// <summary>
    /// Represents the basic contract of a MultiDependencyMetadataOwnerCommand. <para/>
    /// Multi dependency metadata owner commands provide the basic abstract implementation of a multi dependency command that owns a MetadaCollection. 
    /// Metadata collections are a set of configurable data processed by various components of the framework when the command is defined in a 
    /// command container registered in the command manager service of the framework.
    /// </summary>
    /// <typeparam name="TCommand">The type of the dependency command associated with this MultiDependencyCommand.</typeparam>
    /// <typeparam name="TMetadataCollection">The type of the metadata collection this multi dependency command owns.</typeparam>
    /// <typeparam name="TMetadataDefinition">The metadata filter type of the metadata collection owned by this multi dependency command.</typeparam>
    public interface IMultiDependencyMetadataOwnerCommand<TCommand, TMetadataCollection, TMetadataDefinition> : IMultiDependencyCommand<TCommand>
        where TCommand : IDependencyCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        /// <summary>
        /// Represents a set of configurable metadata definitions processed by various components of the framework if the parent multi dependency command
        /// is defined in a command container registered in the command manager of the framework.
        /// </summary>
        TMetadataCollection Metadata { get; }
    }
}
