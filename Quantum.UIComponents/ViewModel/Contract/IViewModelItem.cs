using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.UIComponents
{
    public interface IViewModelItem : IViewModel
    {
        object Value { get; }
        IRootViewModel Root { get; }
        bool IsSelected { get; }
    }
}
