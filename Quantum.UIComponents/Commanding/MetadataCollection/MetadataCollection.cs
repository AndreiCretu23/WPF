using System.Collections;
using System.Collections.Generic;

namespace Quantum.Command
{
    public class MetadataCollection : IEnumerable<IDefinitionMetadata>
    {
        public List<IDefinitionMetadata> internalList { get; private set; } = new List<IDefinitionMetadata>();

        public IEnumerator<IDefinitionMetadata> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        public void Add(IDefinitionMetadata metadataDefinition)
        {
            internalList.Add(metadataDefinition);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return internalList.GetEnumerator();
        }
    }
}
