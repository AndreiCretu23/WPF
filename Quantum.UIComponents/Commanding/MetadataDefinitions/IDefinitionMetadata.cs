using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    public interface IDefinitionMetadata
    {
        /// <summary>
        /// Specifies whether the metadata type supports more than one instance in a collection.
        /// </summary>
        bool SupportsMultiple { get; }
    }
}
