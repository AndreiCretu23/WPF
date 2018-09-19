using Microsoft.Practices.Unity;
using Quantum.CoreModule;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Quantum.UIComponents
{
    public class QuantumToolkitModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterService<ILongOperationService, LongOperationService>();
            
        }
    }
}
