using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    public interface IMetadataAsserterService
    {
        void AssertMetadata<TCommandContainer>() where TCommandContainer : class, ICommandContainer;
        void AssertCommand(object command, string commandContainerName, string commandName);
    }
}
