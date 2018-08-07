using System.Collections.Generic;

namespace Quantum.Metadata
{
    public interface IMetadataAsserterService
    {
        void AssertMetadataCollectionProperties(object obj, string objName = null);

        void AssertMetadataCollection<TCollection, TDefinition>(TCollection collection, string collectionName = null)
            where TDefinition : IMetadataDefinition
            where TCollection : IEnumerable<TDefinition>;
    }
}
