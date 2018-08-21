using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Metadata
{
    [Mandatory(false)]
    [SupportsMultiple(false)]
    [MandatoryCollection(false)]
    public class MainMenuOption : MetadataCollection<IMainMenuMetadata>, ICommandMetadata
    {
    }
}
