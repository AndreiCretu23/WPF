using Microsoft.Practices.Composite.Events;
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
        private string description = Resources.LongOperation_DefaultDescription;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged(() => Description);
            }
        }
        
        public LongOperationViewModel(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
    }
}
