using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Command;
using Quantum.Common;
using Quantum.CoreModule;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.UIComponents;
using Quantum.Utils;
using System.Collections.Generic;
using System.Windows.Input;
using WPF.Commands;
using WPF.Dialogs;
using WPF.Panels;
using WPF.ToolBars;

namespace WPF
{
    public class ApplicationModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterService<IDummyService, DummyService>();

            container.RegisterService<IDynamicPanelSelectionManagerService, DynamicPanelSelectionManagerService>();
            container.Resolve<IDynamicPanelSelectionManagerService>().InitializeDynamicPanelSelection();

            //Menu
            container.Resolve<ICommandManagerService>().RegisterCommandContainer<ICommonCommands, CommonCommands>();

            //ToolBar
            var toolBarManager = container.Resolve<IToolBarManagerService>();
            foreach(var toolBar in GetToolBars(container))
            {
                toolBarManager.RegisterToolBarDefinition(toolBar);
            }

            //PanelDefinitions
            var panelManager = container.Resolve<IPanelManagerService>();
            foreach(var panelDef in GetPanels(container))
            {
                panelManager.RegisterPanelDefinition(panelDef);
            }


            //Dialogs
            var dialogManager = container.Resolve<IDialogManagerService>();
            dialogManager.RegisterAllDefinitions(GetDialogs());
        }

        private IEnumerable<IToolBarDefinition> GetToolBars(IUnityContainer container)
        {
            return new List<IToolBarDefinition>()
            {
                new ToolBarDefinition<ISecondToolBarView, SecondToolBarView, ISecondToolBarViewModel, SecondToolBarViewModel>()
                {
                    Band = 0,
                    BandIndex = 1,
                    Visibility = () => container.Resolve<SelectedNumber>().Value > 5,
                    ToolBarMetadata = new ToolBarMetadataCollection()
                    {
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                    }
                },

                new ToolBarDefinition<IFirstToolBarView, FirstToolBarView, IFirstToolBarViewModel, FirstToolBarViewModel>()
                {
                    Band = 0,
                    BandIndex = 0,
                }
            };
        }

        private IEnumerable<IPanelDefinition> GetPanels(IUnityContainer container)
        {
            yield return new StaticPanelDefinition<IActivePanelView, ActivePanelView, IActivePanelViewModel, ActivePanelViewModel>()
            {
                new StaticPanelConfiguration()
                {
                     CanClose = () => true,
                     CanFloat = () => true,
                     CanOpen = () => true,
                     IsVisible = () => true,
                     Placement = PanelPlacement.Center,
                     Title = () => "ActivePanel"
                },
                new PanelMenuOption()
                {
                    new MenuPath(MenuLocations.View, 0, 0), 
                    new Description("ActivePanel")
                }, 
                new BringIntoViewOnKeyShortcut(ModifierKeys.Control | ModifierKeys.Alt, Key.A),
                new BringIntoViewOnSelection<SelectedNumber, int>()
            };

            yield return new StaticPanelDefinition<IListBoxPanelView, ListBoxPanelView, IListBoxPanelViewModel, ListBoxPanelViewModel>()
            {
                new StaticPanelConfiguration()
                {
                    CanClose = () => true,
                    CanFloat = () => true,
                    CanOpen = () => true,
                    IsVisible = () => true,
                    Placement = PanelPlacement.TopLeft,
                    Title = () => "ListBoxPanel"
                },
                new PanelMenuOption()
                {
                    new MenuPath(MenuLocations.View, categoryIndex: 1, orderIndex: 100),
                    new Description("List Box Panel"),
                },
            };

            yield return new StaticPanelDefinition<IListPanelView, ListPanelView, IListPanelViewModel, ListPanelViewModel>()
            {
                new StaticPanelConfiguration()
                {
                    CanClose = () => true,
                    CanFloat = () => true,
                    CanOpen = () => true,
                    IsVisible = () => false,
                    Placement = PanelPlacement.Center,
                    Title = () => "ListPanel"
                },
                new PanelMenuOption()
                {
                    new MenuPath(MenuLocations.View, categoryIndex: 1, orderIndex: 200),
                    new Description("List Panel"),
                },
            };

            yield return new StaticPanelDefinition<ISelectionPanelView, SelectionPanelView, ISelectionPanelViewModel, SelectionPanelViewModel>()
            {
                new StaticPanelConfiguration()
                {
                    CanClose = () => true,
                    CanFloat = () => true,
                    CanOpen = () => container.Resolve<IEventAggregator>().GetEvent<SelectedNumber>().Value > 5,
                    //IsVisible = () => container.Resolve<IEventAggregator>().GetEvent<SelectedNumber>().Value > 5,
                    Placement = PanelPlacement.TopRight,
                    Title = () => "SelectionPanel"
                },
                new PanelMenuOption()
                {
                    new MenuPath(MenuLocations.View, 0, 1),
                    new Description("SelectionPanel")
                },

                new AutoInvalidateOnSelection<SelectedNumber, int>(),
                new BringIntoViewOnKeyShortcut(ModifierKeys.Control | ModifierKeys.Alt, Key.S),
            };

            yield return new DynamicPanelDefinition<IDynamicPanelView, DynamicPanelView, IDynamicPanelViewModel, DynamicPanelViewModel>()
            {
                new DynamicPanelConfiguration<IDynamicPanelViewModel>()
                {
                    CanFloat = o => true,
                    Title = o => $"DynamicPanel{o.SafeCast<IIdentifiable>().Guid}{container.Resolve<SelectedNumber>().Value.ToString()}",
                    Placement = PanelPlacement.Center,
                },
                new PanelSelectionBinding(typeof(DynamicPanelSelection)),
                new AutoInvalidateOnSelection<SelectedNumber, int>()
            };
        }

        private IEnumerable<IDialogDefinition> GetDialogs()
        {
            yield return new DialogDefinition<ICustomDialogView, CustomDialogView, ICustomDialogViewModel, CustomDialogViewModel>();
        }
    }

    
    
}
