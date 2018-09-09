using Quantum.Command;
using Quantum.Metadata;
using Quantum.Services;
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

namespace WPF.Commands
{
    public class CommonCommands : ServiceBase, ICommonCommands
    {
        [Selection]
        public SelectedNumber Number { get; set; }

        public CommonCommands(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            Number.Value = 3;
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
                        new KeyShortcut(ModifierKeys.Control, Key.N)
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new Description("asd"),
                    }
                }
            };

        public IManagedCommand Yolo1Command =>
            new ManagedCommand()
            {
                CanExecuteHandler = () => true,
                ExecuteHandler = () => { },
                Metadata = new CommandMetadataCollection() {
                    new MainMenuOption()
                    {
                        new MenuPath(MenuLocations.Yolo1, 1, 1),
                        new Description("asd"),
                    }
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                    CanExecuteHandler = () => Number.Value > 4,
                    ExecuteHandler = () => Number.Value = 2,
                    Metadata = new CommandMetadataCollection() {
                        new MainMenuOption()
                        {
                            new MenuPath(MenuLocations.File, 2, 0),
                            new Description("Change2")
                        },
                        new KeyShortcut(ModifierKeys.Control, Key.D2), 
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber)),
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
                                    new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                            new Description("Change5"),
                        },
                        new AutoInvalidateOnSelection(typeof(SelectedNumber)),
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
                        new AutoInvalidateOnSelection(typeof(SelectedNumber))
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
