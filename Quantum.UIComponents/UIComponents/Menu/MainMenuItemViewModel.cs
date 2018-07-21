using Quantum.Command;
using Quantum.UIComposition;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Quantum.UIComponents
{
    public class MainMenuItemViewModel : ObservableObject, IMainMenuItemViewModel
    {
        public IMenuEntry MenuEntry { get; private set; }

        public MainMenuItemViewModel(IMenuEntry menuEntry) {
            MenuEntry = menuEntry;
        }

        #region Properties

        private ICommand command;
        public ICommand Command
        {
            get { return command; }
            set
            {
                command = value;
                RaisePropertyChanged(() => Command);
            }
        }

        private string icon;
        public string Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                RaisePropertyChanged(() => Icon);
            }
        }

        private bool isCheckable;
        public bool IsCheckable
        {
            get { return isCheckable; }
            set
            {
                isCheckable = value;
                RaisePropertyChanged(() => IsCheckable);
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                RaisePropertyChanged(() => IsChecked);
                CheckedChanged?.Invoke(value);
            }
        }

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

        private string tooltip;
        public string ToolTip
        {
            get { return tooltip; }
            set
            {
                tooltip = value;
                RaisePropertyChanged(() => ToolTip);
            }
        }
        
        private string shortcut;
        public string Shortcut
        {
            get { return shortcut; }
            set
            {
                shortcut = value;
                RaisePropertyChanged(() => Shortcut);
            }
        }

        public Action<bool> CheckedChanged { get; set; }

        #endregion Properties

        #region Children

        private Func<MainMenuItemViewModel, IEnumerable<IMainMenuItemViewModel>> childrenDelegate;
        public Func<MainMenuItemViewModel, IEnumerable<IMainMenuItemViewModel>> ChildrenDelegate
        {
            get { return childrenDelegate; }
            set
            {
                childrenDelegate = value;
                RaiseChildrenChanged();
            }
        }

        public IEnumerable<IMainMenuItemViewModel> Children { get { return ChildrenDelegate(this); } }

        public void RaiseChildrenChanged()
        {
            RaisePropertyChanged(() => Children);
        }

        #endregion Children
    }
}
