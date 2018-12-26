using Quantum.Utils;
using System;
using System.Windows;
using System.Windows.Media;

namespace Quantum.Controls
{
    public class VisualTraverser : IVisualTraverser
    {
        public void Traverse(DependencyObject root, Func<DependencyObject, TraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            root.AssertParameterNotNull(nameof(root));

            filter = filter ?? (o => TraverseBehavior.Continue);
            targetAction = targetAction ?? (o => { });

            TraversePrivate(root, filter, targetAction);
        }


        private TraverseBehavior TraversePrivate(DependencyObject element, Func<DependencyObject, TraverseBehavior> filter, Action<DependencyObject> targetAction)
        {
            var behavior = filter(element);
            
            if(behavior == TraverseBehavior.Stop) {
                return behavior;
            }

            targetAction(element);

            if(behavior != TraverseBehavior.ContinueSkipChildren) {
                var lastChildIndex = VisualTreeHelper.GetChildrenCount(element) - 1;
                for(int i = lastChildIndex; i >= 0; i--) {
                    var child = VisualTreeHelper.GetChild(element, i);
                    var result = TraversePrivate(child, filter, targetAction);
                    if(result == TraverseBehavior.Stop) {
                        return result;
                    }
                }
            }

            return behavior;
        }
    }
}
