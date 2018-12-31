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
using System.Windows.Input;

namespace WPF.Panels
{
    [Guid("00D8BA2E-ED54-45A6-8098-0FC992C35627")]
    public class SourcePanelViewModel : ViewModelBase, ISourcePanelViewModel
    {
        public IEnumerable<ViewModelItem> Children { get { return CreateChildren(); } }

        public bool IsSelecting { get; set; }

        public SourcePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        private IEnumerable<ViewModelItem> GetChildrenInternal(string head)
        {
            yield return new ViewModelItem() { Header = head + "1" };
            yield return new ViewModelItem() { Header = head + "2" , ChildrenGetter = GetChildrenInternal };
            yield return new ViewModelItem() { Header = head + "2" };
            yield return new ViewModelItem() { Header = head + "3" };
            yield return new ViewModelItem() { Header = head + "4" , ChildrenGetter = GetChildrenInternal };
        }

        private IEnumerable<ViewModelItem> CreateChildren()
        {
            for(int i = 0; i < 500; i++) {
                yield return new ViewModelItem()
                {
                    Header = i.ToString(),
                    ChildrenGetter = GetFunc(i),
                };
            }

        }

        private Func<string, IEnumerable<ViewModelItem>> GetFunc(int number)
        {
            if(number % 5 == 0) {
                return GetChildrenInternal;
            }

            return null;
        }

    }

    public class ViewModelItem : ObservableObject
    {
        public static int InstanceCount = 0;

        private string header;
        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                RaisePropertyChanged(() => Header);
            }
        }

        private bool isEditing;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                RaisePropertyChanged(() => IsEditing);
            }
        }

        private string editableHeader;
        public string EditableHeader
        {
            get { return editableHeader; }
            set
            {
                editableHeader = value;
                RaisePropertyChanged(() => EditableHeader);
            }
        }

        public IEnumerable<KeyBinding> VMIShortcuts
        {
            get
            {
                return new Collection<KeyBinding>()
                {
                    new KeyBinding()
                    {
                        Command = TreeViewItemCommands.Rename,
                        CommandParameter = this,
                        Modifiers = ModifierKeys.None,
                        Key = Key.F2, 
                    }
                };
            }
        }

        public ViewModelItem()
        {
            InstanceCount++;
        }

        public Func<string, IEnumerable<ViewModelItem>> ChildrenGetter { get; set; }

        private IEnumerable<ViewModelItem> CreateChildren()
        {
            if(ChildrenGetter != null) {
                return ChildrenGetter(header);
            }

            else {
                return Enumerable.Empty<ViewModelItem>();
            }
        }

        public IEnumerable<ViewModelItem> Children { get { return CreateChildren(); } }
    }
    
    public static class TreeViewItemCommands
    {
        public static IDelegateCommand<ViewModelItem> Rename
        {
            get
            {
                return new DelegateCommand<ViewModelItem>()
                {
                    ExecuteHandler = vm =>
                    {
                        PropertyChangedEventHandler renameHandler = null;
                        renameHandler = (sender, e) =>
                        {
                            if (e.PropertyName == ReflectionUtils.GetPropertyName((ViewModelItem o) => o.IsEditing)) {
                                vm.Header = vm.EditableHeader;
                                ((INotifyPropertyChanged)sender).PropertyChanged -= renameHandler;
                            }
                        };

                        vm.EditableHeader = vm.Header;
                        vm.IsEditing = true;
                        vm.PropertyChanged += renameHandler;
                    }
                };
            }
        }
    }
}
