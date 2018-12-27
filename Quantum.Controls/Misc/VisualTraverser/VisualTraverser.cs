using Quantum.Utils;
using System;
using System.Windows;
using System.Windows.Media;

namespace Quantum.Controls
{
    public class VisualTraverser : IVisualTraverser
    {
        public void Traverse(DependencyObject root, Func<DependencyObject, VisualTraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            root.AssertParameterNotNull(nameof(root));

            filter = filter ?? (o => VisualTraverseBehavior.Continue | VisualTraverseBehavior.TraverseChildren | VisualTraverseBehavior.Process);
            targetAction = targetAction ?? (o => { });

            TraversePrivate(root, filter, targetAction);
        }


        private VisualTraverseBehavior TraversePrivate(DependencyObject element, Func<DependencyObject, VisualTraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            var behavior = filter(element);
            
            if(behavior.HasFlag(VisualTraverseBehavior.Process)) {
                targetAction(element);
            }

            if (!behavior.HasFlag(VisualTraverseBehavior.Continue)) {
                return behavior;
            }

            if(behavior.HasFlag(VisualTraverseBehavior.TraverseChildren)) {
                var lastChildIndex = VisualTreeHelper.GetChildrenCount(element) - 1;
                for(int i = lastChildIndex; i >= 0; i--) {
                    var child = VisualTreeHelper.GetChild(element, i);
                    var result = TraversePrivate(child, filter, targetAction);
                    if(!result.HasFlag(VisualTraverseBehavior.Continue)) {
                        return result;
                    }
                }
            }

            return behavior;
        }
    }
}
