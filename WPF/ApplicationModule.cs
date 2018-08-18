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
using System.Windows;
using System.Windows.Input;
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


            container.Resolve<ICommandManagerService>().RegisterCommandContainer<CommonCommands>();


            var toolBarManager = container.Resolve<IToolBarManagerService>();
            foreach(var toolBar in GetToolBars(container))
            {
                toolBarManager.RegisterToolBarDefinition(toolBar);
            }


            var panelManager = container.Resolve<IPanelManagerService>();
            foreach(var panelDef in GetPanels(container))
            {
                panelManager.RegisterPanelDefinition(panelDef);
            }


            var selection = container.Resolve<IEventAggregator>().GetEvent<DynamicPanelSelection>();
            for (int i = 1; i <= 5; i++)
            {
                selection.Value.Add(new DynamicPanelViewModel(container.Resolve<IObjectInitializationService>())
                {
                    Guid = i.ToString(),
                    DisplayText = $"DynamicPanel {i.ToString()}"
                });
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

    public static class MenuLocations
    {
        public static readonly AbstractMenuPath File = new AbstractMenuPath(AbstractMenuPath.Root, new Description("File"), 0, 1);
        public static readonly AbstractMenuPath Edit = new AbstractMenuPath(AbstractMenuPath.Root, new Description("Edit"), 0, 2);
        public static readonly AbstractMenuPath View = new AbstractMenuPath(AbstractMenuPath.Root, new Description("View"), 0, 3);

        public static readonly AbstractMenuPath Category3To4 = new AbstractMenuPath(File, new Description("3To4"), 1, 1);
        public static readonly AbstractMenuPath Yolo1 = new AbstractMenuPath(Category3To4, new Description("Yolo1"), 1, 4);
        public static readonly AbstractMenuPath Yolo2 = new AbstractMenuPath(Category3To4, new Description("Yolo"), 2, 1);
        public static readonly AbstractMenuPath SubCat3To4 = new AbstractMenuPath(Category3To4, new Description("SubCategory"), 1, 3);
        public static readonly AbstractMenuPath Recent = new AbstractMenuPath(File, new Description("Recent"), 2, 1);

        public static readonly AbstractMenuPath Category8To10 = new AbstractMenuPath(Edit, new Description("8To10"), 1, 2);
        
    }

    public class CommonCommands : QuantumServiceBase, ICommandContainer
    {
        [Selection]
        public SelectedNumber Number { get; set; }

        public CommonCommands(IObjectInitializationService initSvc)
            : base (initSvc)
        {
            Number.Value = 3;
        }

        public ManagedCommand Qwerty
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true,
                    ExecuteHandler = () => { Container.Resolve<IToolBarManagerService>().RestoreLayout(); },
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.SubCat3To4, 0, 0), 
                        new Description("qwerty"), 
                    }
                };
            }
        }

        public ManagedCommand Yolo2Command =>
            new ManagedCommand()
            {
                CanExecuteHandler = () => true, 
                ExecuteHandler = () => { }, 
                MainMenuMetadata = new MenuMetadataCollection() {
                    new MenuPath(MenuLocations.Yolo2, 1, 1), 
                    new Description("asd"), 
                }
            };


        public ManagedCommand Yolo1Command =>
            new ManagedCommand()
            {
                CanExecuteHandler = () => true,
                ExecuteHandler = () => { },
                MainMenuMetadata = new MenuMetadataCollection() {
                    new MenuPath(MenuLocations.Yolo1, 1, 1),
                    new Description("asd"),
                }
            };

        public ManagedCommand Change1 {
            get {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value > 3,
                    ExecuteHandler = () => Number.Value = 1,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    }, 
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.File, 1, 0), 
                        new Description("Change1"), 
                        new Checkable(false), 
                        new KeyShortcut(ModifierKeys.Control, Key.D1)
                    }
                };
            }
        }

        public ManagedCommand Cheange2 {
            get {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value > 4,
                    ExecuteHandler = () => Number.Value = 2,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    }, 
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.File, 2, 0), 
                        new Description("Change2"), 
                        new KeyShortcut(ModifierKeys.Control, Key.D1)
                    }
                };
            }
        }

        public ManagedCommand Change3 {
            get {
                return new ManagedCommand() {
                    CanExecuteHandler = () => Number.Value > 5,
                    ExecuteHandler = () => Number.Value = 3,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.Category3To4, 0, 0),
                        new Description("Change3"),
                        new KeyShortcut(ModifierKeys.Control, Key.D3)
                    }
                };
            }
        }

        public ManagedCommand Change4
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value > 6,
                    ExecuteHandler = () => Number.Value = 4,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.Category3To4, 0, 1),
                        new Description("Change4"),
                        new KeyShortcut(ModifierKeys.Control, Key.D4)
                    }
                };
            }
        }

        public ManagedCommand Change5
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true,
                    ExecuteHandler = () => Number.Value = 5,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber)),
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.File, 2, 0),
                        new Description("Change5"),
                    }
                };
            }
        }

        public MultiManagedCommand RecentCommands
        {
            get
            {
                return new MultiManagedCommand()
                {
                    SubCommands = () =>
                    {
                        var collection = new SubCommandCollection();
                        for(int i = 0; i < Number.Value; i++) {
                            collection.Add(new SubCommand()
                            {
                                CanExecuteHandler = () => true,
                                ExecuteHandler = () => MessageBox.Show(i.ToString()),
                                CommandMetadata = new CommandMetadataCollection() {
                                    new AutoInvalidateOnSelection(typeof(SelectedNumber))
                                },
                                SubCommandMetadata = new SubMenuMetadataCollection() {
                                    new Description($"Print {i.ToString()}"),
                                }
                            });
                        }
                        return collection;
                    },
                    MenuMetadata = new MultiMenuMetadataCollection()
                    {
                        new MenuPath(MenuLocations.Recent, 0, 0), 
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    }
                };
            }
        }

        //public ManagedCommandCollection RecentCommands
        //{
        //    get
        //    {
        //        var collection = new ManagedCommandCollection();
        //        for(int i = 0; i < Number.Value; i++) {
        //            collection.Add(new ManagedCommand()
        //            {
        //                CanExecuteHandler = () => true, 
        //                ExecuteHandler = () => MessageBox.Show(i.ToString()),
        //                CommandMetadata = new CommandMetadataCollection() {
        //                    new AutoInvalidateOnSelection(typeof(SelectedNumber))
        //                }, 
        //                MainMenuMetadata = new MenuMetadataCollection() {
        //                    new MenuPath(MenuLocations.Recent, 0, i), 
        //                    new Description($"Print {i.ToString()}"), 
        //                }
        //            });
        //        }
        //        collection.CommandCollectionMetadata = new CommandCollectionMetadataCollection() {
        //            new AutoInvalidateOnSelection(typeof(SelectedNumber))
        //        };
        //        return collection;
        //    }
        //}

        public ManagedCommand Change6
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 4,
                    ExecuteHandler = () => Number.Value = 6,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber)),
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.File, 2, 0),
                        new Description("Change5"),
                    }
                };
            }
        }


        public ManagedCommand Change7
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 5,
                    ExecuteHandler = () => Number.Value = 7,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.Edit, 1, 1),
                        new Description("Change7"),
                        new KeyShortcut(ModifierKeys.Control, Key.D7)
                    }
                };
            }
        }

        public ManagedCommand Change8
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 6,
                    ExecuteHandler = () => Number.Value = 8,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.Category8To10, 1, 0), 
                        new Description("Change8"), 
                        new KeyShortcut(ModifierKeys.Control, Key.D8)
                    }
                };
            }
        }

        public ManagedCommand Change9
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 7,
                    ExecuteHandler = () => Number.Value = 9,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.Category8To10, 2, 0),
                        new Description("Change9"),
                        new KeyShortcut(ModifierKeys.Control, Key.D9)
                    }
                };
            }
        }

        public ManagedCommand Change10
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 8,
                    ExecuteHandler = () => Number.Value = 10,
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    },
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.Category8To10, 2, 1),
                        new Description("Change10"),
                    }
                };
            }
        }

        public ManagedCommand CheckCommand
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true, 
                    ExecuteHandler = () => { }, 
                    CommandMetadata = new CommandMetadataCollection() {
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
                    }, 
                    MainMenuMetadata = new MenuMetadataCollection() {
                        new MenuPath(MenuLocations.Edit, 3, 0),
                        new Description("Check Me"), 
                        new Checkable(true), 
                        new CheckChanged(o => MessageBox.Show(o ? "Yey" : "Nay")),
                    }
                };
            }
        }


    }

}
