using Quantum.Services;
using Quantum.UIComponents;
using Quantum.UIComposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Panels
{
    [Guid("50924313-BDE0-41D6-A0D5-C8E86CA1EF00")]
    public class TreePanelViewModel : ViewModelBase, ITreePanelViewModel
    {
        public IEnumerable<TreePanelVMI> Children { get { return CreateChildren(); } }

        public TreePanelViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        public IEnumerable<TreePanelVMI> CreateChildren()
        {
            return Enumerable.Range(1, 10).Select(o => new TreePanelVMI(o)
            {
                ChildrenGetter = n => Enumerable.Range(n * 10, (n + 1) * 10 - 1).Select(i => new TreePanelVMI(i)).ToList()
            }).ToList();
        }
    }



    public class TreePanelVMI : ObservableObject
    {
        public int Number { get; }
        public string NumberDisplay { get { return Number.ToString(); } }
        public Func<int, IEnumerable<TreePanelVMI>> ChildrenGetter { get; set; }
        public IEnumerable<TreePanelVMI> Children { get { return ChildrenGetter?.Invoke(Number) ?? Enumerable.Empty<TreePanelVMI>(); } }

        public TreePanelVMI(int number)
        {
            Number = number;
        }

    }
}
