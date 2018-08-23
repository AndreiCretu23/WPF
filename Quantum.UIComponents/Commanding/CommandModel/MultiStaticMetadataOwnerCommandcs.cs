using Quantum.Metadata;

namespace Quantum.Command
{
    public class MultiStaticMetadataOwnerCommand<TCommand, TMetadataCollection, TMetadataDefinition> : MultiStaticCommand<TCommand>, IMultiStaticMetadataOwnerCommand<TCommand, TMetadataCollection, TMetadataDefinition>
         where TCommand : IStaticCommand
         where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
         where TMetadataDefinition : IMetadataDefinition
    {
        public TMetadataCollection Metadata { get; set; } = new TMetadataCollection();
    }
}
