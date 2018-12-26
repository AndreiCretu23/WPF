using System;
using System.Windows;

namespace Quantum.Controls
{
    public interface IVisualTraverser
    {
        void Traverse(DependencyObject root, Func<DependencyObject, TraverseBehavior> filter, Action<DependencyObject> targetAction);
    }

    public enum TraverseBehavior
    {
        Continue, 
        ContinueSkipChildren,
        Stop
    }
}
