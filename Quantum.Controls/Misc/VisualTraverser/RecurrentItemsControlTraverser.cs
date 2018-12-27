using System;
using System.Windows;
using System.Windows.Controls;

namespace Quantum.Controls
{
    public class RecurrentItemsControlTraverser : IVisualTraverser
    {
        public void Traverse(DependencyObject root, Func<DependencyObject, VisualTraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            if (!(root is ItemsControl)) {
                throw new Exception("Error : ItemsControlTraverser can only be used on an ItemsControl root element.");
            }

            filter = filter ?? (o => VisualTraverseBehavior.Continue | VisualTraverseBehavior.TraverseChildren | VisualTraverseBehavior.Process);
            targetAction = targetAction ?? (o => { });

            var itemsControl = (ItemsControl)root;

            TraversePrivate(itemsControl, filter, targetAction);
        }

        private VisualTraverseBehavior TraversePrivate(ItemsControl element, Func<DependencyObject, VisualTraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            var behavior = filter(element);

            if (behavior.HasFlag(VisualTraverseBehavior.Process)) {
                targetAction(element);
            }

            if (!behavior.HasFlag(VisualTraverseBehavior.Continue)) {
                return behavior;
            }
            
            if (behavior.HasFlag(VisualTraverseBehavior.TraverseChildren)) {
                for (int i = element.Items.Count; i >= 0; i--) {
                    var child = element.ItemContainerGenerator.ContainerFromIndex(i);
                    if(child is ItemsControl childItemsControl) {
                        var result = TraversePrivate(childItemsControl, filter, targetAction);
                        if (!result.HasFlag(VisualTraverseBehavior.Continue)) {
                            return result;
                        }
                    }
                }
            }

            return behavior;
        }
    }
}
