using Microsoft.Practices.Unity;
using Quantum.CoreModule;
using Quantum.Services;
using Quantum.Command;
using Quantum.Metadata;
using Quantum.Shortcuts;

namespace Quantum.UIComponents
{
    public class QuantumUIModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            //ObjectInitializationExtensions
            container.Resolve<IObjectInitializationService>().RegisterInitializer<InvalidationInitializer>();

            //Metadata
            container.RegisterService<IMetadataAsserterService, MetadataAsserterService>();

            //Command
            container.RegisterService<ICommandInvalidationManagerService, CommandInvalidationManagerService>();
            container.RegisterService<ICommandManagerService, CommandManagerService>();
            container.Resolve<IObjectInitializationService>().RegisterInitializer<CommandInitializer>();

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

            //Shortcut Manager
            container.RegisterService<IShortcutManagerService, ShortcutManagerService>();

            //DialogManager
            container.RegisterService<IDialogManagerService, DialogManagerService>();


            //Services & ToolKit
            container.RegisterService<ILongOperationService, LongOperationService>();
            container.RegisterService<IIconManagerService, IconManagerService>();

            //View Core
            container.RegisterService<IUICoreService, UICoreService>();
        }
    }
}
