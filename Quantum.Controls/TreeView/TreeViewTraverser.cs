using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UIItemsControl = System.Windows.Controls.ItemsControl;

namespace Quantum.Controls
{
    internal static class TreeViewTraverser
    {
        internal static TreeViewItem DetermineUpperTreeViewItem(TreeViewItem first, TreeViewItem second)
        {
            first.AssertParameterNotNull(nameof(first));
            second.AssertParameterNotNull(nameof(second));

            var firstDescendantChain = first.GetThisAndAncestors().OfType<TreeViewItem>().Reverse().ToArray();
            var secondDescendantChain = second.GetThisAndAncestors().OfType<TreeViewItem>().Reverse().ToArray();

            var minCount = System.Math.Min(firstDescendantChain.Length, secondDescendantChain.Length);
            for(int i = 0; i < minCount; i++) {
                if (firstDescendantChain[i] == secondDescendantChain[i]) continue;

                var commonParent = firstDescendantChain[i].Parent;
                if(commonParent.ItemContainerGenerator.IndexFromContainer(firstDescendantChain[i]) < 
                   commonParent.ItemContainerGenerator.IndexFromContainer(secondDescendantChain[i])) {
                    return first;
                }
                
                else {
                    return second;
                }
            }

            if(firstDescendantChain.Length > secondDescendantChain.Length) {
                return second;
            }

            return first;
        }
        
        internal static IList<TreeViewItem> GetItemsBetween(TreeViewItem item1, TreeViewItem item2)
        {
            item1.AssertParameterNotNull(nameof(item1));
            item2.AssertParameterNotNull(nameof(item2));
            
            var result = new List<TreeViewItem>();

            var upper = DetermineUpperTreeViewItem(item1, item2);
            var lower = (upper == item1) ? item2 : item1;
            
            while(true) {
                result.Add(upper);

                if(upper == lower) break;

                upper = upper.GetNext();

                if(upper == null) break;
            }
            
            return result;
        }

        internal static IEnumerable<UIItemsControl> GetThisAndAncestors(this TreeViewItem treeViewItem)
        {
            UIItemsControl parent = treeViewItem;
            while (parent != null) {
                yield return parent;
                parent = parent is TreeViewItem tvi ? tvi.Parent : null;
            }
        }

        internal static IEnumerable<TreeViewItem> GetChildren(this TreeViewItem treeViewItem)
        {
            treeViewItem.AssertParameterNotNull(nameof(treeViewItem));

            for (int i = 0; i < treeViewItem.Items.Count; i++) {
                if (treeViewItem.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem item) {
                    yield return item;
                }
            }
        }

        internal static TreeViewItem GetPrevious(this TreeViewItem treeViewItem)
        {
            treeViewItem.AssertParameterNotNull(nameof(treeViewItem));

            TreeViewItem getBottomChild(TreeViewItem relativeTo)
            {
                var children = relativeTo.GetChildren().Where(o => o.IsVisible);
                if (children.Any()) {
                    return getBottomChild(children.Last());
                }
                return relativeTo;
            }

            var currentIndex = treeViewItem.Parent.ItemContainerGenerator.IndexFromContainer(treeViewItem);
            
            if (currentIndex == 0) {
                if (treeViewItem.Parent is TreeViewItem item) {
                    return item;
                }
                else {
                    return null;
                }
            }
            
            else
            {
                var prevContainer = (TreeViewItem)treeViewItem.Parent.ItemContainerGenerator.ContainerFromIndex(currentIndex - 1);
                return getBottomChild(prevContainer);
            }
        }

        internal static TreeViewItem GetNext(this TreeViewItem treeViewItem)
        {
            treeViewItem.AssertParameterNotNull(nameof(treeViewItem));

            if (treeViewItem.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem firstChild && firstChild.IsVisible) {
                return firstChild;
            }

            else if (treeViewItem.Parent.ItemContainerGenerator.ContainerFromIndex(treeViewItem.Parent.ItemContainerGenerator.IndexFromContainer(treeViewItem) + 1) is TreeViewItem nextElement && nextElement.IsVisible) {
                return nextElement;
            }

            else if (treeViewItem.Parent is TreeViewItem parentTreeViewItem && parentTreeViewItem.Parent != null) {
                var nextUpperElement = parentTreeViewItem.Parent.ItemContainerGenerator.ContainerFromIndex(parentTreeViewItem.Parent.ItemContainerGenerator.IndexFromContainer(parentTreeViewItem) + 1) as TreeViewItem;
                if(nextUpperElement != null && nextUpperElement.IsVisible) {
                    return nextUpperElement;
                }
            }

            return null;
        }

    }
}
