using System;
using System.Windows;

namespace Quantum.Controls
{
    public interface IVisualTraverser
    {
        void Traverse(DependencyObject root, Func<DependencyObject, VisualTraverseBehavior> filter, Action<DependencyObject> targetAction);
    }

    [Flags]
    public enum VisualTraverseBehavior
    {
        None = 0,
        Continue = 1, 
        TraverseChildren = 2,
        Process = 4
    }
}
