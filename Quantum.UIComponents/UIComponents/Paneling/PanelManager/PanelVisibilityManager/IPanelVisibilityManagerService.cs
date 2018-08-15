using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal interface IPanelVisibilityManagerService
    {
        void SetLayoutGroupData(IDictionary<LayoutAnchorable, LayoutAnchorablePane> layoutData);

        void SetVisibility(LayoutAnchorable anchorable, bool visibility);
    }
}
