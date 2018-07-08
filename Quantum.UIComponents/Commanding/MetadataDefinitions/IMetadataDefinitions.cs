using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    public interface IMetadataDefinition
    {
    }
    
    public interface ICommandMetadata : IMetadataDefinition
    {
    }
    
    public interface IMenuMetadata : IMetadataDefinition
    {
    }
    
    public interface IMultiMenuMetadata : IMetadataDefinition
    {
    }

    public interface ISubMenuMetadata : IMetadataDefinition
    {
    }
}
