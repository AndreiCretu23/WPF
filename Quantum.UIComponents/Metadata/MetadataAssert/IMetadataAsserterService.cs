using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Metadata
{
    public interface IMetadataAsserterService
    {
        void AssertMetadataCollections(object obj, string objName = null);
    }
}
