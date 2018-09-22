using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Command;
using Quantum.Core;
using Quantum.CoreModule;
using Quantum.Events;
using System;
using System.Collections.Generic;

namespace WPF
{
    public class Bootstrapper : QuantumBootstrapper
    {
        protected override IEnumerable<IQuantumModule> GetApplicationModules()
        {
            yield return new ApplicationModule();
        }
        
    }
}
