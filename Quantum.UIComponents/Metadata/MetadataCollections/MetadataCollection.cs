using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Quantum.Metadata
{   /// <summary>
    /// The base class for all metadata collections. Metadata collections are a set of configurable metadata 
    /// definitions attached to an owner processed by various components of the framework.
    /// </summary>
    /// <typeparam name="TDefinition"></typeparam>
    public abstract class MetadataCollection<TDefinition> : IEnumerable<TDefinition>
        where TDefinition : IMetadataDefinition
    {
        private Collection<TDefinition> InternalCollection { get; set; } = new Collection<TDefinition>();

        public IEnumerator<TDefinition> GetEnumerator() {
            return InternalCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return InternalCollection.GetEnumerator();
        }

        public void Add(TDefinition metadataDefinition) {
            InternalCollection.Add(metadataDefinition);
        }
    }

    
}
