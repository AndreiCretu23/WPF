using Quantum.Metadata;

namespace Quantum.Command
{
    /// <summary>
    /// Defines the basic abstract implementation of a static command that owns a MetadaCollection. Metadata collections 
    /// are a set of configurable data processed by various components of the framework when the command is defined in a 
    /// command container registered in the command manager service of the framework.
    /// </summary>
    /// <typeparam name="TMetadataCollection">The type of the metadata collection this static command owns.</typeparam>
    /// <typeparam name="TMetadataDefinition">The metadata filter type of the metadata collection owned by this static command.</typeparam>
    public abstract class StaticMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition> : StaticCommand, IStaticMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition>
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        /// <summary>
        /// Represents a set of configurable metadata definitions processed by various components of the framework if the parent static command
        /// is defined in a command container registered in the command manager of the framework.
        /// </summary>
        public TMetadataCollection Metadata { get; set; } = new TMetadataCollection();
    }
}
