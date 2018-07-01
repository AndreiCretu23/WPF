using System.Collections;
using System.Collections.Generic;

namespace Quantum.Command
{
    public abstract class MetadataCollection<TDefinition> : IEnumerable<TDefinition>
        where TDefinition : IMetadataDefinition
    {
        private List<TDefinition> InternalCollection { get; set; } = new List<TDefinition>();

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
