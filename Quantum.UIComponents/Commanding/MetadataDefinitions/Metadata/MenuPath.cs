namespace Quantum.Command
{
    public class AbstractMenuPath
    {
        public AbstractMenuPath(AbstractMenuPath parentPath, int categoryIndex, int orderIndex, Icon icon = null)
        {
            ParentPath = parentPath;
            CategoryIndex = categoryIndex;
            OrderIndex = orderIndex;
            Icon = icon;
        }

        public AbstractMenuPath ParentPath { get; private set; }
        public int CategoryIndex { get; private set; }
        public int OrderIndex { get; private set; }
        public Icon Icon { get; private set; }
    }

    public class MenuPath : IMainMenuMetadata
    {
        public MenuPath(AbstractMenuPath parentPath, int categoryIndex, int orderIndex)
        {
            ParentPath = parentPath;
            CategoryIndex = categoryIndex;
            OrderIndex = orderIndex;
        }

        public bool SupportsMultiple { get { return false; } }
        public AbstractMenuPath ParentPath { get; private set; }
        public int CategoryIndex { get; private set; }
        public int OrderIndex { get; private set; }
    }
}
