using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    public interface IMetadataDefinition
    {
        bool SupportsMultiple { get; }
    }
    
    public interface ICommandMetadata : IMetadataDefinition
    {
    }

    public interface IMainMenuMetadata : IMetadataDefinition
    {
    }
}
