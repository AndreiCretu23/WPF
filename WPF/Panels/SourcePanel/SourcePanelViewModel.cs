using Quantum.Command;
using Quantum.Services;
using Quantum.UIComponents;
using Quantum.UIComposition;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF.Panels
{
    [Guid("00D8BA2E-ED54-45A6-8098-0FC992C35627")]
    public class SourcePanelViewModel : ViewModelBase, ISourcePanelViewModel
    {
        public SourcePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public IEnumerable<ContextMenuItemViewModel> ContextMenuItems
        {
            get
            {
                var s = DateTime.Now.Second;
                for (int i = 0; i < s; i++)
                {
                    yield return new ContextMenuItemViewModel()
                    {
                        Header = i.ToString()
                    };
                }
            }
        }

        public IEnumerable<KeyBinding> Shortcuts
        {
            get
            {
                var second = DateTime.Now.Second;
                if (second < 30)
                {
                    yield return new KeyBinding()
                    {
                        Modifiers = ModifierKeys.Control,
                        Key = Key.N,
                        Command = new DelegateCommand()
                        {
                            CanExecuteHandler = () => true,
                            ExecuteHandler = () => MessageBox.Show($"First : {DateTime.Now.Second.ToString()}")
                        }
                    };
                }
                else
                {
                    yield return new KeyBinding()
                    {
                        Modifiers = ModifierKeys.Control,
                        Key = Key.N,
                        Command = new DelegateCommand()
                        {
                            CanExecuteHandler = () => true,
                            ExecuteHandler = () => MessageBox.Show($"Second : {DateTime.Now.Second.ToString()}")
                        }
                    };
                }
            }
        }
    }

    public class ContextMenuItemViewModel
    {
        public string Header { get; set; }
    }

}
