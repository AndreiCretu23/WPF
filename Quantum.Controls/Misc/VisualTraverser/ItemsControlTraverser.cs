using System;
using System.Windows;
using System.Windows.Controls;

namespace Quantum.Controls
{
    public class ItemsControlTraverser : IVisualTraverser
    {
        public void Traverse(DependencyObject root, Func<DependencyObject, TraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            if(!(root is ItemsControl)) {
                throw new Exception("Error : ItemsControlTraverser can only be used on an ItemsControl root element.");
            }
            
            filter = filter ?? (o => TraverseBehavior.Continue);
            targetAction = targetAction ?? (o => { });

            var itemsControl = (ItemsControl)root;

            var lastChildIndex = itemsControl.Items.Count - 1;
            for(int i = lastChildIndex; i >= 0; i--) {
                var child = itemsControl.ItemContainerGenerator.ContainerFromIndex(i);

                if (child == null) continue;

                if (filter(child) == TraverseBehavior.Stop) {
                    return;
                }

                targetAction(child);
            }

        }
    }
}
