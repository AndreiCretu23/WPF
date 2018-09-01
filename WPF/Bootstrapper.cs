using Quantum.Core;
using Quantum.CoreModule;
using System.Collections.Generic;

namespace WPF
{
    public class Bootstrapper : QuantumBootstrapper
    {
        public override IEnumerable<IQuantumModule> GetApplicationModules()
        {
            yield return new ApplicationModule();
        }
    }
}
