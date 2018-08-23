﻿using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Command;
using Quantum.Common;
using Quantum.CoreModule;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.UIComponents;
using Quantum.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPF.Commands;
using WPF.Panels;
using WPF.ToolBars;

namespace WPF
{
    public class ApplicationModule : IQuantumModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterService<IDummyService, DummyService>();
            container.Resolve<IDummyService>().TestMethod();

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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
            };

            yield return new StaticPanelDefinition<ISelectionPanelView, SelectionPanelView, ISelectionPanelViewModel, SelectionPanelViewModel>()
            {
                new StaticPanelConfiguration()
                {
                    CanClose = () => true,
                    CanFloat = () => true,
                    CanOpen = () => container.Resolve<SelectedNumber>().Value > 5,
                    IsVisible = () => container.Resolve<SelectedNumber>().Value > 5,
                    Placement = PanelPlacement.TopRight,
                    Title = () => "SelectionPanel"
                },
                new PanelMenuOption()
                {
                    new MenuPath(MenuLocations.View, 0, 1),
                    new Description("SelectionPanel")
                },

                new AutoInvalidateOnSelection(typeof(SelectedNumber))
            };

            yield return new DynamicPanelDefinition<IDynamicPanelView, DynamicPanelView, IDynamicPanelViewModel, DynamicPanelViewModel>()
            {
                new DynamicPanelConfiguration<IDynamicPanelViewModel>()
                {
                    CanFloat = o => true,
                    Title = o => $"DynamicPanel{o.SafeCast<IIdentifiable>().Guid}",
                    Placement = PanelPlacement.Center,
                },
                new PanelSelectionBinding(typeof(DynamicPanelSelection))
            };
        }

    }

    
    
}
