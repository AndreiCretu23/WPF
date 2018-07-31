using Microsoft.Practices.Unity;
using Quantum.CoreModule;
using Quantum.Services;
using Quantum.Command;
using Quantum.Metadata;

namespace Quantum.UIComponents
{
    public class QuantumUIModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {

            container.RegisterService<IMetadataAsserterService, MetadataAsserterService>();
            container.RegisterService<ICommandMetadataProcessorService, CommandMetadataProcessorService>();
            container.RegisterService<ICommandManagerService, CommandManagerService>();

            container.RegisterService<IToolBarManagerService, ToolBarManagerService>();

            container.RegisterService<IDockingView, DockingView>();
            container.RegisterService<ShellView>();
            container.RegisterService<ShellViewModel>();
            container.RegisterService<IUICoreService, UICoreService>();
        }
    }
}
