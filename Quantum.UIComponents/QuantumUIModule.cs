using Microsoft.Practices.Unity;
using Quantum.CoreModule;
using Quantum.Services;
using Quantum.UIComponents.Shell;

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
