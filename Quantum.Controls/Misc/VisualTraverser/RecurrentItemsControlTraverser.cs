using System;
using System.Windows;
using System.Windows.Controls;

namespace Quantum.Controls
{
    public class RecurrentItemsControlTraverser : IVisualTraverser
    {
        public void Traverse(DependencyObject root, Func<DependencyObject, TraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            if (!(root is ItemsControl)) {
                throw new Exception("Error : ItemsControlTraverser can only be used on an ItemsControl root element.");
            }

            filter = filter ?? (o => TraverseBehavior.Continue);
            targetAction = targetAction ?? (o => { });

            var itemsControl = (ItemsControl)root;

            TraversePrivate(itemsControl, filter, targetAction);
        }

        private TraverseBehavior TraversePrivate(ItemsControl element, Func<DependencyObject, TraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            var behavior = filter(element);

            if (behavior == TraverseBehavior.Stop) {
                return behavior;
            }

            targetAction(element);

            if (behavior != TraverseBehavior.ContinueSkipChildren) {
                for (int i = element.Items.Count; i >= 0; i--) {
                    var child = element.ItemContainerGenerator.ContainerFromIndex(i);
                    if(child is ItemsControl childItemsControl) {
                        var result = TraversePrivate(childItemsControl, filter, targetAction);
                        if (result == TraverseBehavior.Stop) {
                            return result;
                        }
                    }
                }
            }

            return behavior;
        }
    }
}
