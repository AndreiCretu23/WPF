using Quantum.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Quantum.Metadata
{
    /// <summary>
    /// Provides the basic contract for any menu entry definition inside the application 
    /// that is processed by the framework.
    /// </summary>
    public interface IMenuEntry
    {
        /// <summary>
        /// The instance of the AbstractMenuPath associated with the parent menu entry.
        /// </summary>
        AbstractMenuPath ParentPath { get; }

        /// <summary>
        /// An index indicating in which category will the menu entry be located.
        /// </summary>
        int CategoryIndex { get; }

        /// <summary>
        /// An index indicating the position of the menu entry inside it's category.
        /// </summary>
        int OrderIndex { get; }
    }

    /// <summary>
    /// An abstract menu path represents the definition associated to a menu entry that contains child menu entries.
    /// The main menu / context menus of the application are built from an AbstractMenuPath tree-like structure.
    /// It is recommended all abstract menu paths (both main menu and context menu ones) are defined in a static class
    /// as static readonly fields. The menu paths will look for their parent path by reference, so having unique 
    /// instances for these objects is a must.
    /// </summary>
    public class AbstractMenuPath : IMenuEntry
    {
        /// <summary>
        /// An AbstractMenuPath instance representing the root of a menu.
        /// </summary>
        public static readonly AbstractMenuPath Root = new AbstractMenuPath(null, null, 0, 0, null);

        /// <summary>
        /// Creates a new instance of the AbstractMenuPath class.
        /// </summary>
        /// <param name="parentPath">The instance of the AbstractMenuPath associated with the parent menu entry.</param>
        /// <param name="description">The description associated with the menu entry.</param>
        /// <param name="categoryIndex">An index indicating in which category will the menu entry be located.</param>
        /// <param name="orderIndex">An index indicating the position of the menu entry inside it's category.</param>
        /// <param name="toolTip">The tooltip associated with the menu entry.</param>
        /// <param name="icon">The icon associated with the menu entry.</param>
        public AbstractMenuPath(AbstractMenuPath parentPath, Description description, int categoryIndex, int orderIndex, ToolTip toolTip = null, Icon icon = null)
        {
            ParentPath = parentPath;
            Description = description;
            ToolTip = toolTip;
            CategoryIndex = categoryIndex;
            OrderIndex = orderIndex;
            Icon = icon;
        }

        /// <summary>
        /// The instance of the AbstractMenuPath associated with the parent menu entry.
        /// </summary>
        public AbstractMenuPath ParentPath { get; private set; }

        /// <summary>
        /// The description associated with the menu entry.
        /// </summary>
        public Description Description { get; private set; }

        /// <summary>
        /// The tooltip associated with the menu entry.
        /// </summary>
        public ToolTip ToolTip { get; private set; }

        /// <summary>
        /// An index indicating in which category will the menu entry be located.
        /// </summary>
        public int CategoryIndex { get; private set; }

        /// <summary>
        /// An index indicating the position of the menu entry inside it's category.
        /// </summary>
        public int OrderIndex { get; private set; }

        /// <summary>
        /// The icon associated with the menu entry.
        /// </summary>
        public Icon Icon { get; private set; }

        /// <summary>
        /// Returns all the abstract menu paths from the current instance to the root (excluding the root instance).
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// This metadata type is used to define the location of a menu entry.
    /// </summary>
    [Mandatory(true)]
    [SupportsMultiple(false)]
    public class MenuPath : IAssertable, IMainMenuMetadata, IMultiMainMenuMetadata, IPanelMenuEntryMetadata, IMenuEntry
    {
        /// <summary>
        /// Creates a new instance of the MenuPath class.
        /// </summary>
        /// <param name="parentPath">The instance of the AbstractMenuPath associated with the parent menu entry.</param>
        /// <param name="categoryIndex">An index indicating in which category will the menu entry be located.</param>
        /// <param name="orderIndex">An index indicating the position of the menu entry inside it's category.</param>
        public MenuPath(AbstractMenuPath parentPath, int categoryIndex, int orderIndex)
        {
            ParentPath = parentPath;
            CategoryIndex = categoryIndex;
            OrderIndex = orderIndex;
        }

        /// <summary>
        /// The instance of the AbstractMenuPath associated with the parent menu entry.
        /// </summary>
        public AbstractMenuPath ParentPath { get; private set; }

        /// <summary>
        /// An index indicating in which category will the menu entry be located.
        /// </summary>
        public int CategoryIndex { get; private set; }

        /// <summary>
        /// An index indicating the position of the menu entry inside it's category.
        /// </summary>
        public int OrderIndex { get; private set; }

        /// <summary>
        /// Validates that the AbstractMenuPath associated with the parent menu entry is not null.
        /// </summary>
        /// <param name="objName"></param>
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
