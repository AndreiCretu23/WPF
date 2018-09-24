using System.Collections.Generic;

namespace Quantum.Metadata
{
    /// <summary>
    /// This service is responsible for validating metadata collections and objects containing metadata collection properties.
    /// </summary>
    public interface IMetadataAsserterService
    {
        /// <summary>
        /// Validates all MetadataCollection properties of an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objName"></param>
        void AssertMetadataCollectionProperties(object obj, string objName = null);

        /// <summary>
        /// Validates a metadata collection and throws an exception if the data is not valid, explaning the input mistake.
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <typeparam name="TDefinition"></typeparam>
        /// <param name="collection"></param>
        /// <param name="collectionName"></param>
        void AssertMetadataCollection<TCollection, TDefinition>(TCollection collection, string collectionName = null)
            where TDefinition : IMetadataDefinition
            where TCollection : IEnumerable<TDefinition>;
    }
}
