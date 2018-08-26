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
            container.RegisterService<IPanelLayoutManagerService, PanelLayoutManagerService>();
            container.RegisterService<IStaticPanelVisibilityManagerService, StaticPanelVisibilityManagerService>();
            container.RegisterService<IStaticPanelProcessingService, StaticPanelProcessingService>();
            container.RegisterService<IDynamicPanelProcessingService, DynamicPanelProcessingService>();
            container.RegisterService<IPanelProcessingService, PanelProcessingService>();

            //View Components
            container.RegisterService<IDockingView, DockingView>();
            container.RegisterService<ShellView>();
            container.RegisterService<ShellViewModel>();


            //DialogManager
            container.RegisterService<IDialogManagerService, DialogManagerService>();


            //View Core
            container.RegisterService<IUICoreService, UICoreService>();
        }
    }
}
