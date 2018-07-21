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

            container.RegisterService<IMetadataAsserterService, MetadataAsserterService>();
            container.RegisterService<ICommandMetadataProcessorService, CommandMetadataProcessorService>();
            container.RegisterService<ICommandManagerService, CommandManagerService>();

            
            container.RegisterService<ShellView>();
            container.RegisterService<ShellViewModel>();
            container.RegisterService<IUICoreService, UICoreService>();
        }
    }
}
