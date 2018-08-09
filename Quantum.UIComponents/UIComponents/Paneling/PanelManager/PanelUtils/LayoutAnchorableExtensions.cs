using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Quantum.UIComponents
{
    internal static class LayoutAnchorableExtensions
    {
        internal static void SetVisibility(this LayoutAnchorable layoutAnchorable, bool visibility)
        {
            layoutAnchorable.AssertNotNull(nameof(layoutAnchorable));
            try
            {
                if(visibility && layoutAnchorable.IsAutoHidden)
                {
                    layoutAnchorable.ToggleAutoHide();
                }
                else if(!visibility && !layoutAnchorable.IsAutoHidden)
                {
                    layoutAnchorable.ToggleAutoHide();
                }
            }
            catch
            {
                //Do nothing. Means the layout anchorable has not been loaded yet in the UI.
            }
        }
    }
}
