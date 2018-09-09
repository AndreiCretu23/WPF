using Quantum.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Quantum.Metadata
{
    public interface IMenuEntry
    {
        AbstractMenuPath ParentPath { get; }
        int CategoryIndex { get; }
        int OrderIndex { get; }
        int Depth { get; }
    }

    public class AbstractMenuPath : IMenuEntry
    {
        public static readonly AbstractMenuPath Root = new AbstractMenuPath(null, null, 0, 0, null);

        public AbstractMenuPath(AbstractMenuPath parentPath, Description description, int categoryIndex, int orderIndex, ToolTip toolTip = null, Icon icon = null)
        {
            ParentPath = parentPath;
            Description = description;
            ToolTip = toolTip;
            CategoryIndex = categoryIndex;
            OrderIndex = orderIndex;
            Icon = icon;
        }

        public AbstractMenuPath ParentPath { get; private set; }
        public Description Description { get; private set; }
        public ToolTip ToolTip { get; private set; }
        public int CategoryIndex { get; private set; }
        public int OrderIndex { get; private set; }
        public Icon Icon { get; private set; }
        public int Depth { get { return GetPathsToRoot().Count(); } }

        public IEnumerable<AbstractMenuPath> GetPathsToRoot() {
            if (AbstractMenuPath.Root == this) yield break;

            var menuPath = this;
            do
            {
                yield return menuPath;
                menuPath = menuPath.ParentPath;
            } while (menuPath != AbstractMenuPath.Root);
        }
    }

    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class MenuPath : IAssertable, IMainMenuMetadata, IMultiMainMenuMetadata, IPanelMenuEntryMetadata, IMenuEntry
    {
        public MenuPath(AbstractMenuPath parentPath, int categoryIndex, int orderIndex)
        {
            ParentPath = parentPath;
            CategoryIndex = categoryIndex;
            OrderIndex = orderIndex;
        }
        
        public AbstractMenuPath ParentPath { get; private set; }
        public int CategoryIndex { get; private set; }
        public int OrderIndex { get; private set; }
        public int Depth { get { return ParentPath.Depth + 1; } }

        [DebuggerHidden]
        public void Assert(string objName = null)
        {
            if(ParentPath == null)
            {
                throw new Exception($"Error : {objName ?? String.Empty} has a MenuPath metadata definition that has a null ParentPath.");
            }
        }
    }
}
