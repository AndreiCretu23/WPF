using System.Collections.Generic;
using System.Linq;

namespace Quantum.Command
{
    internal interface IMenuEntry
    {
        AbstractMenuPath ParentPath { get; }
        int CategoryIndex { get; }
        int OrderIndex { get; }
        int Depth { get; }
    }

    public class AbstractMenuPath : IMenuEntry
    {
        public static readonly AbstractMenuPath Root = new AbstractMenuPath(null, null, 0, 0, null);

        public AbstractMenuPath(AbstractMenuPath parentPath, Description description, int categoryIndex, int orderIndex, Icon icon = null)
        {
            ParentPath = parentPath;
            Description = description;
            CategoryIndex = categoryIndex;
            OrderIndex = orderIndex;
            Icon = icon;
        }

        public AbstractMenuPath ParentPath { get; private set; }
        public Description Description { get; private set; }
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
    public class MenuPath : IMenuMetadata, IMultiMenuMetadata, IMenuEntry
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
    }
}
