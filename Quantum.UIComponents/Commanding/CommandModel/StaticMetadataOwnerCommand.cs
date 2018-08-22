using Quantum.Metadata;

namespace Quantum.Command
{
    public abstract class StaticMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition> : StaticCommand, IStaticMetadataOwnerCommand<TMetadataCollection, TMetadataDefinition>
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        public TMetadataCollection Metadata { get; set; } = new TMetadataCollection();
    }
}
