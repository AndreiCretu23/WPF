using Quantum.Core;
using Quantum.CoreModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF
{
    public class Bootstrapper : QuantumBootstrapper
    {
        public override IEnumerable<IQuantumModule> GetApplicationModules()
        {
            yield break;
        }
    }
}
