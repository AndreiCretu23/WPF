using Quantum.Services;
using Quantum.UIComponents;
using Quantum.UIComposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace WPF.Panels
{
    [Guid("50924313-BDE0-41D6-A0D5-C8E86CA1EF00")]
    public class TreePanelViewModel : TreeViewModel, ITreePanelViewModel
    {
        private Timer Timer { get; }
        private IList<Wrapper> Wrappers { get; }

        [Selection]
        public SelectedNumber SelectedNumber { get; set; }

        public TreePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
            ExpansionRetainingStrategy = TreeExpansionRetainingStrategy.ItemPath;
            // AllowMultipleSelection = false;
            // SetSelectionBinding(SelectedNumber);

            Wrappers = Enumerable.Range(1, 999).Select(o => new Wrapper { Value = o }).ToList();

            Timer = new Timer()
            {
                Interval = 5000,
                AutoReset = true,
                Enabled = true,
            };

            Timer.Elapsed += OnTimerElapsed;
            Timer.Start();
        }

        public void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            InvalidateChildren();
            int b = 3;
        }
        
        public override IEnumerable<ITreeViewModelItem> CreateContentItems()
        {
            //return Wrappers.Where(o => o.Digits == 1).OrderBy(o => o.Value).Select(o => new TreeViewModelItem<Wrapper>(o)
            //{
            //    HeaderGetter = w => w.Value.ToString(),
            //    ChildrenGetter = w => Wrappers.Where(n => n.Digits == 2 && n.FirstDigit == w.FirstDigit).OrderBy(n => n.Value).Select(n => new TreeViewModelItem<Wrapper>(n)
            //    {
            //        HeaderGetter = sw => sw.Value.ToString(),
            //        ChildrenGetter = sw => Wrappers.Where(m => m.Digits == 3 && m.FirstDigit == sw.FirstDigit).OrderBy(m => m.Value).Select(m => new TreeViewModelItem<Wrapper>(m)
            //        {
            //            HeaderGetter = ssw => ssw.Value.ToString()
            //        })
            //    })
            //});


            return Enumerable.Range(0, 10).Select(o => new TreeViewModelItem<int>(o)
            {
                HeaderGetter = n => n.ToString(),
                ChildrenGetter = n => Enumerable.Range(n * 10, (n + 1) * 10).Select(c => new TreeViewModelItem<int>(c)
                {
                    HeaderGetter = nr => nr.ToString()
                })
            });
        }

    }


    public class Wrapper
    {
        public int Value { get; set; }

        public int Digits { get { return Convert.ToInt32(Math.Floor(Math.Log10(Value) + 1)); } }

        public int FirstDigit => Convert.ToInt32(Math.Floor(Value / Math.Pow(10, Digits - 1)));

    }
    
}
