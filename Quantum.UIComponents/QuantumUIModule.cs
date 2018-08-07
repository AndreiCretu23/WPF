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
            //Metadata
            container.RegisterService<IMetadataAsserterService, MetadataAsserterService>();

            //Command
            container.RegisterService<ICommandMetadataProcessorService, CommandMetadataProcessorService>();
            container.RegisterService<ICommandManagerService, CommandManagerService>();
            
            //ToolBar
            container.RegisterService<IToolBarManagerService, ToolBarManagerService>();

            //Docking
            container.RegisterService<IPanelManagerService, PanelManagerService>();

            //View Components
            container.RegisterService<IDockingView, DockingView>();
            container.RegisterService<ShellView>();
            container.RegisterService<ShellViewModel>();

            //View Core
            container.RegisterService<IUICoreService, UICoreService>();
        }
    }
}
