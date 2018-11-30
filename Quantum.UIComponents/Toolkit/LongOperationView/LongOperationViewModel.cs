using Microsoft.Practices.Composite.Events;
using Quantum.CoreModule;
using Quantum.Events;
using Quantum.ResourceLibrary;
using Quantum.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Quantum.UIComponents
{
    internal class LongOperationViewModel : ViewModelBase, ILongOperationViewModel
    {
        [Selection]
        public SelectedLongOperationDescription SelectedDescription { get; set; }

        [InvalidateOn(typeof(SelectedLongOperationDescription))]
        public string Description { get { return SelectedDescription.Value; } }

        public LongOperationViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
    }
}
