using Quantum.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
{
    public interface ICommandManagerService
    {

    }

    internal class CommandManagerService : QuantumServiceBase, ICommandManagerService
    {
        public CommandManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
    }
}
