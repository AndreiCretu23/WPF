using Quantum.CoreModule;
using Quantum.Services;
using Quantum.UIComponents.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Quantum.UIComponents
{
    public class QuantumUIModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterType<ShellView>(new ContainerControlledLifetimeManager());
            container.RegisterType<ShellViewModel>(new ContainerControlledLifetimeManager());

            container.RegisterService<IUICoreService, UICoreService>();
        }
    }
}
