using Quantum.Metadata;

namespace Quantum.Command
{
    public class MultiDependencyMetadataOwnerCommand<T, TCommand, TMetadataCollection, TMetadataDefinition> : MultiDependencyCommand<T, TCommand>, IMultiDependencyMetadataOwnerCommand<TCommand, TMetadataCollection, TMetadataDefinition>
        where T : class
        where TCommand : DependencyCommand<T>
        where TMetadataCollection : MetadataCollection<TMetadataDefinition>, new()
        where TMetadataDefinition : IMetadataDefinition
    {
        public TMetadataCollection Metadata { get; set; } = new TMetadataCollection();
    }
}
