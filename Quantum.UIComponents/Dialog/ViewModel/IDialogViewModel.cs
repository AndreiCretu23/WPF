using Quantum.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    public interface IDialogViewModel
    {
        IDelegateCommand AbortCommand { get; }
        IDelegateCommand SaveCommand { get; }
    }
}
