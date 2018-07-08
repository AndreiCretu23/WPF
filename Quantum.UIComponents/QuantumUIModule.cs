using Microsoft.Practices.Unity;
using Quantum.CoreModule;
using Quantum.Services;
using Quantum.Command;

namespace Quantum.UIComponents
{
    public class QuantumUIModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterService<ShellView>();
            container.RegisterService<ShellViewModel>();
            container.RegisterService<IMetadataAsserterService, MetadataAsserterService>();
            container.RegisterService<ICommandMetadataProcessorService, CommandMetadataProcessorService>();
            container.RegisterService<ICommandManagerService, CommandManagerService>();
            container.RegisterService<IMenuManagerService, MenuManagerService>();

            container.RegisterService<IUICoreService, UICoreService>();
        }
    }
}
