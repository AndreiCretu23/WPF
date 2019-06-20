using Quantum.Command;
using Quantum.Services;
using Quantum.UIComponents;
using Quantum.UIComposition;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF.Panels
{
    [Guid("00D8BA2E-ED54-45A6-8098-0FC992C35627")]
    public class SourcePanelViewModel : ListViewModel, ISourcePanelViewModel
    {
        [Selection]
        public SelectedNumber SelectedNumber { get; set; }

        private int CurrentCount { get; set; } = 2;

        public SourcePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            //AllowMultipleSelection = true;
            //SetSelectionBinding(new MultipleObjectSelection());
            SetupTimer();

            //AllowMultipleSelection = false;
            //SyncItems = false;
            //SetSelectionBinding(SelectedNumber);
        }

        private void SetupTimer()
        {
            var t = new Timer()
            {
                AutoReset = true,
                Enabled = true,
                Interval = 1000,
            };

            t.Elapsed += (sender, e) =>
            {
                CurrentCount++;
                InvalidateChildren();
                if(CurrentCount > 25)
                {
                    t.Stop();
                }
            };
        }

        protected override IEnumerable<IListViewModelItem> CreateContentItems()
        {
            if(CurrentCount > 10)
            {
                AllowMultipleSelection = false;
                if(HasBoundSelection) {
                    BreakSelectionBinding();
                }
                SetSelectionBinding(SelectedNumber);
            }
            
            for(int i = 0; i < CurrentCount; i++)
            {
                yield return new ListViewModelItem<int>(i % 5)
                {
                    HeaderGetter = o => o.ToString()
                };
            }
            
        }
    }
}
