using Quantum.Metadata;

namespace Quantum.Command
{
    /// <summary>
    /// Defines the basic abstract implementation of a multi dependency command that owns a MetadaCollection. Metadata collections 
    /// are a set of configurable data processed by various components of the framework when the command is defined in a 
    /// command container registered in the command manager service of the framework.
    /// </summary>
    /// <typeparam name="T">The type of the parameter of this multi dependency command</typeparam>
    /// <typeparam name="TCommand">The type of the dependency command associated with this MultiDependencyCommand.</typeparam>
    /// <typeparam name="TMetadataCollection">The type of the metadata collection this multi dependency command owns.</typeparam>
    /// <typeparam name="TMetadataDefinition">The metadata filter type of the metadata collection owned by this multi dependency command.</typeparam>
    public class MultiDependencyMetadataOwnerCommand<T, TCommand, TMetadataCollection, TMetadataDefinition> : MultiDependencyCommand<T, TCommand>, IMultiDependencyMetadataOwnerCommand<TCommand, TMetadataCollection, TMetadataDefinition>
        where T : class
        where TCommand : IDependencyCommand
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        /// <summary>
        /// Represents a set of configurable metadata definitions processed by various components of the framework if the parent multi dependency command
        /// is defined in a command container registered in the command manager of the framework.
        /// </summary>
        public TMetadataCollection Metadata { get; set; } = new TMetadataCollection();
    }
}
