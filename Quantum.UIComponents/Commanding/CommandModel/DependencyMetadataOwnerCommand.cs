using Quantum.Metadata;

namespace Quantum.Command
{
    public abstract class DependencyMetadataOwnerCommand<T, TMetadataCollection, TMetadataDefinition> : DependencyCommand<T>, IDependencyMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition>
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        public TMetadataCollection Metadata { get; set; } = new TMetadataCollection();
    }
}
