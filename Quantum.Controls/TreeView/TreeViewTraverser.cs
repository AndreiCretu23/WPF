using Quantum.Utils;
using System.Collections.Generic;
using System.Linq;

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
    }
}
