using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Shortcuts;
using Quantum.UIComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF.Dialogs;
using WPF.Panels;

namespace WPF.Commands
{
    public class CommonCommands : ServiceBase, ICommonCommands
    {
        [Selection]
        public SelectedNumber Number { get; set; }

        [Selection]
        public DynamicPanelSelection DynamicPanelSelection { get; set; }

        [Service]
        public ICommandManagerService CommandManager { get; set; }

        [Service]
        public IPanelManagerService PanelManager { get; set; }

        [Service]
        public IShortcutManagerService ShortcutManager { get; set; }

        public CommonCommands(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }


        public IManagedCommand OpenDialog
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true,
                    ExecuteHandler = () =>
                    {
                        var dialogManager = Container.Resolve<IDialogManagerService>();
                        var result = dialogManager.ShowDialog<ICustomDialogViewModel>();
                        if (result == true)
                        {
                            MessageBox.Show("True Dialog Result");
                        }

                        else if (result == false)
                        {
                            MessageBox.Show("False Dialog Result");
                        }

                        else
                        {
                            MessageBox.Show("Null Dialog Result");
                        }
                    },
                    Metadata = new CommandMetadataCollection()
                    {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 0, 0),
                            new Description("Open Dialog"),
                            new Icon("/Quantum.ResourceLibrary;component/Icons/Common/appbar.adobe.aftereffects.png"),
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.N),
                        new CommandGuid("F6FB6107-D1CA-4C04-B1EE-8D31BC37454A"),
                    }
                };
            }
        }
        
        public IManagedCommand RemoveLastPanel
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true,
                    ExecuteHandler = () =>
                    {
                        var lastValue = DynamicPanelSelection.Value.LastOrDefault();
                        if(lastValue != null) {
                            DynamicPanelSelection.Remove(lastValue);
                        }
                    },
                    Metadata = new CommandMetadataCollection()
                    {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 0, 100),
                            new Description("Remove DPanel"),
                        },

                        new KeyShortcut(ModifierKeys.Control, Key.R),
                        new CommandGuid("67520506-1500-4D5A-AAE7-E27F765E438A"),
                    }
                };
            }
        }



        public IManagedCommand CommandChangeNShortcut
        {
            get
            {
                return new ManagedCommand()
                {
                    ExecuteHandler = () =>
                    {
                        var command = CommandManager.GetCommand((ICommonCommands c) => c.OpenDialog);
                        ShortcutManager.SetShortcut(command, ModifierKeys.Control, Key.N);
                    },

                    Metadata = new CommandMetadataCollection()
                    {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 0, 200),
                            new Description("Command Change N"),
                        },
                        new CommandGuid("8B4C3024-374D-492A-A78C-54ADF46DDAB6"),
                    }
                };
            }
        }

        public IManagedCommand CommandChangeBShortcut
        {
            get
            {
                return new ManagedCommand()
                {
                    ExecuteHandler = () =>
                    {
                        var command = CommandManager.GetCommand((ICommonCommands c) => c.OpenDialog);
                        ShortcutManager.SetShortcut(command, ModifierKeys.Control, Key.B);
                    },

                    Metadata = new CommandMetadataCollection()
                    {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 0, 201),
                            new Description("Command Change B"),
                        },
                        new CommandGuid("65D479E2-7342-4118-B5D0-1FA354BC380D"),
                    }
                };
            }
        }

        public IManagedCommand CommandClearShortcut
        {
            get
            {
                return new ManagedCommand()
                {
                    ExecuteHandler = () =>
                    {
                        var command = CommandManager.GetCommand((ICommonCommands c) => c.OpenDialog);
                        ShortcutManager.ClearShortcut(command);
                    },

                    Metadata = new CommandMetadataCollection()
                    {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 0, 202),
                            new Description("Command Clear Shortcut"),
                        },
                        new CommandGuid("CDC8E94A-49C9-4C1B-9CEE-F7F952C66A1A"),
                    }
                };
            }
        }



        public IManagedCommand PanelChangeAShortcut
        {
            get
            {
                return new ManagedCommand()
                {
                    ExecuteHandler = () =>
                    {
                        var def = PanelManager.GetStaticPanelDefinition<IActivePanelViewModel>();
                        ShortcutManager.SetShortcut(def, ModifierKeys.Control | ModifierKeys.Alt, Key.A);
                    },

                    Metadata = new CommandMetadataCollection()
                    {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 0, 250),
                            new Description("Panel Change A"),
                        },
                        new CommandGuid("F5D36683-11B9-4585-8BB8-AECC5F546959"),
                    }
                };
            }
        }

        public IManagedCommand PanelChangeQShortcut
        {
            get
            {
                return new ManagedCommand()
                {
                    ExecuteHandler = () =>
                    {
                        var def = PanelManager.GetStaticPanelDefinition<IActivePanelViewModel>();
                        ShortcutManager.SetShortcut(def, ModifierKeys.Control | ModifierKeys.Alt, Key.Q);
                    },

                    Metadata = new CommandMetadataCollection()
                    {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 0, 251),
                            new Description("Panel Change Q"),
                        },
                        new CommandGuid("C9AC8CE7-A484-41EB-A48D-950719E039B1"),
                    }
                };
            }
        }

        public IManagedCommand PanelClearShortcut
        {
            get
            {
                return new ManagedCommand()
                {
                    ExecuteHandler = () =>
                    {
                        var def = PanelManager.GetStaticPanelDefinition<IActivePanelViewModel>();
                        ShortcutManager.ClearShortcut(def);
                    },

                    Metadata = new CommandMetadataCollection()
                    {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 0, 252),
                            new Description("Panel Clear Shortcut"),
                        },
                        new CommandGuid("36434E28-219D-486A-A415-250FB65E506A"),
                    }
                };
            }
        }





        public IManagedCommand Qwerty
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true,
                    ExecuteHandler = () => { Container.Resolve<IToolBarManagerService>().RestoreLayout(); },
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.SubCat3To4, 0, 0),
                            new Description("qwerty"),
                        },
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("2079C609-5303-4290-B46A-A3D641361F07"),
                    },
                };
            }
        }

        public IManagedCommand Yolo2Command =>
            new ManagedCommand()
            {
                CanExecuteHandler = () => true,
                ExecuteHandler = () => { },
                Metadata = new CommandMetadataCollection() {
                    new MainMenuOption()
                    {
                        new MenuPath(MenuLocations.Yolo2, 1, 1),
                        new Description("Yolo2"),
                    },
                    new CommandGuid("0CCACCB6-169F-4F60-A3C4-4F3520709477"),
                }
            };

        public IManagedCommand Yolo1Command =>
            new ManagedCommand()
            {
                CanExecuteHandler = () => EventAggregator.GetEvent<DynamicPanelSelection>().Value.Any(),
                ExecuteHandler = () => 
                {
                    var selection = EventAggregator.GetEvent<DynamicPanelSelection>();
                    var last = selection.Value.Last();
                    selection.Remove(last);
                },
                Metadata = new CommandMetadataCollection() {
                    new MainMenuOption()
                    {
                        new MenuPath(MenuLocations.Yolo1, 1, 1),
                        new Description("Yolo1"),
                    }, 
                    new AutoInvalidateOnSelection<DynamicPanelSelection, IEnumerable<IDynamicPanelViewModel>>(),
                    new CommandGuid("221125A9-2FA8-49F4-A9FA-02654E88A200"),
                }
            };
        
        public IManagedCommand Change1
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value > 3,
                    ExecuteHandler = () => Number.Value = 1,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 1, 0),
                            new Description("Change1"),
                            new Checkable(false),
                            new Icon("/Quantum.ResourceLibrary;component/Icons/Common/appbar.camera.flash.selected.png")
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.D1),
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("6A518A86-DEFD-46D7-922F-94C941DC7B97"),
                    },
                };
            }
        }

        public IManagedCommand Change2
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true,//Number.Value > 4,
                    ExecuteHandler = () => Number.Value = 2,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 2, 0),
                            new Description("Change2")
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.D2),
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("168ED7AB-6B7F-40F6-B1D5-14ABAEE90D1D"),
                    },
                };
            }
        }

        public IManagedCommand Change3
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value > 5,
                    ExecuteHandler = () => Number.Value = 3,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.Category3To4, 0, 0),
                            new Description("Change3")
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.D3),
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("9756C01B-34CF-472B-A16F-A4BE3E464811"),
                    }
                };
            }
        }

        public IManagedCommand Change4
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value > 6,
                    ExecuteHandler = () => Number.Value = 4,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.Category3To4, 0, 1),
                            new Description("Change4")
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.D4),
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("B4D85D20-3ACE-4392-BD63-E307831AEDDB"),
                    }
                };
            }
        }

        public IManagedCommand Change5
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true,
                    ExecuteHandler = () => Number.Value = 5,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 2, 0),
                            new Description("Change5"),
                        },
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("159ED877-1B2F-4338-B25C-1D46DCF2634F")
                    }
                };
            }
        }

        public IMultiManagedCommand RecentCommands
        {
            get
            {
                return new MultiManagedCommand()
                {
                    Commands = () =>
                    {
                        var collection = new Collection<ISubCommand>();
                        for (int i = 0; i < Number.Value; i++)
                        {
                            collection.Add(new SubCommand()
                            {
                                CanExecuteHandler = () => true,
                                ExecuteHandler = () => MessageBox.Show(i.ToString()),
                                Metadata = new SubCommandMetadataCollection()
                                {
                                    new SubMainMenuOption()
                                    {
                                        new Description($"Print {i.ToString()}"), 
                                        new ToolTip($"{i.ToString()}")
                                    },
                                   new AutoInvalidateOnSelection<SelectedNumber, int>()
                                }
                            });
                        }
                        return collection;
                    },

                    Metadata = new MultiCommandMetadataCollection()
                    {
                        new MultiMainMenuOption()
                        {
                            new MenuPath(MenuLocations.Recent, 0, 0)
                        },
                        new AutoInvalidateOnSelection<SelectedNumber, int>()
                    }

                };
            }
        }

        public IMultiManagedCommand SubRecentCommands
        {
            get
            {
                return new MultiManagedCommand()
                {
                    Commands = () =>
                    {
                        var collection = new Collection<ISubCommand>();
                        foreach(var panel in DynamicPanelSelection.Value) {
                            collection.Add(new SubCommand()
                            {
                                CanExecuteHandler = () => Int32.TryParse(panel.DisplayText, out Int32 number) && number < 5,
                                ExecuteHandler = () => MessageBox.Show(panel.DisplayText),
                                Metadata = new SubCommandMetadataCollection()
                                {
                                    new SubMainMenuOption()
                                    {
                                        new Description($"Show {panel.DisplayText}"),
                                        new ToolTip($"{panel.DisplayText}")
                                    },
                                }
                            });
                        }
                        return collection;
                    },

                    Metadata = new MultiCommandMetadataCollection()
                    {
                        new MultiMainMenuOption()
                        {
                            new MenuPath(MenuLocations.SubRecent, 0, 0)
                        },
                        new AutoInvalidateOnSelection<SelectedNumber, int>()
                    }
                };
            }
        }

        public IManagedCommand Change6
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 4,
                    ExecuteHandler = () => Number.Value = 6,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 2, 0),
                            new Description("Change6"),
                        },
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("E447751D-7ECE-43BA-8736-9C67E5093F2C"),
                    }
                };
            }
        }

        public IManagedCommand Change7
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 5,
                    ExecuteHandler = () => Number.Value = 7,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.Edit, 1, 1),
                            new Description("Change7")
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.D7),
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("178DF1F5-70F2-4682-951D-5DEBFA0548E2"),
                    }
                };
            }
        }

        public IManagedCommand Change8
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 6,
                    ExecuteHandler = () => Number.Value = 8,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.Category8To10, 1, 0),
                            new Description("Change8")
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.D8),
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("EEF55F74-4850-498B-B02B-A6B0C37337CC"),
                    }
                };
            }
        }

        public IManagedCommand Change9
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 7,
                    ExecuteHandler = () => Number.Value = 9,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.Category8To10, 2, 0),
                            new Description("Change9")
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.D9),
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("490E5F3C-43C5-4642-88AE-809EB8B94BB8"),
                    }
                };
            }
        }

        public IManagedCommand Change10
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value < 8,
                    ExecuteHandler = () => Number.Value = 10,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.Category8To10, 2, 1),
                            new Description("Change10"),
                        },
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("E35E88BF-6496-4459-98BA-1776796D4976"),
                    }
                };
            }
        }

        public IManagedCommand CheckCommand
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => true,
                    ExecuteHandler = () => { },
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.Edit, 3, 0),
                            new Description("Check Me"),
                            new Checkable(true),
                            new CheckChanged(o => MessageBox.Show(o ? "Yey" : "Nay")),
                        },
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("0584D896-63B5-46EA-9413-617ADC3E6C87"),
                    }
                };
            }
        }


        public IManagedCommand HiddenCommand
        {
            get
            {
                return new ManagedCommand()
                {
                    CanExecuteHandler = () => Number.Value != 7,
                    ExecuteHandler = () => MessageBox.Show("Hidden Command"),

                    Metadata = new CommandMetadataCollection()
                    {
                        new KeyShortcut(ModifierKeys.Control, Key.H),
                        new AutoInvalidateOnSelection<SelectedNumber, int>(),
                        new CommandGuid("F27015BA-A83E-4EE7-8FD2-3511119B6293"),
                    }
                };
            }
        }
        

        public IMultiManagedCommand SpamCommand
        {
            get
            {
                return new MultiManagedCommand()
                {
                    Commands = () =>
                    {
                        var subCommands = new List<ISubCommand>();
                        var numberRange = Enumerable.Range(0, 200);
                        foreach(var i in numberRange)
                        {
                            subCommands.Add(new SubCommand()
                            {
                                CanExecuteHandler = () => true,
                                ExecuteHandler = () => MessageBox.Show($"Spam {i.ToString()}"),
                                Metadata = new SubCommandMetadataCollection()
                                {
                                    new SubMainMenuOption()
                                    {
                                        new Description($"SpamEntry {i.ToString()}")
                                    }
                                }
                            });
                        }
                        return subCommands;
                    }, 

                    Metadata = new MultiCommandMetadataCollection()
                    {
                        new MultiMainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 100, 0), 
                        },
                    }
                };
            }
        }

    }

}
