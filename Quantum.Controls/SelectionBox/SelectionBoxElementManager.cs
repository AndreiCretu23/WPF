using Quantum.Math;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Quantum.Controls
{
    public class SelectionBoxElementManager
    {
        public FrameworkElement Owner { get; }
        public SelectionBox SelectionBox { get; }

        public Type TargetType { get { return SelectionBox.TargetType; } }
        public DependencyProperty TargetSelectionProperty { get { return SelectionBox.TargetSelectionProperty; } }
        public IVisualTraverser VisualTraverser { get { return SelectionBox.VisualTraverser; } }

        private ISet<FrameworkElement> SelectedItems { get; set; }
        private bool IsActiveDragging { get; set; }

        public SelectionBoxElementManager(FrameworkElement owner, SelectionBox selectionBox)
        {
            Owner = owner;
            SelectionBox = selectionBox;
        }

        

        public void BeginSelection()
        {
            SelectedItems = new HashSet<FrameworkElement>();
        }

        public void UpdateSelection(Rect selectionRectangle)
        {
            if(!IsActiveDragging) {
                ClearSelection();
                IsActiveDragging = true;
            }

            var selectedItems = new HashSet<FrameworkElement>();
            var passedSelectionBox = false;
            FrameworkElement firstAboveElement = null;

            VisualTraverser.Traverse
            (
                root: Owner,
                filter: o =>
                {
                    if (!(o is FrameworkElement element) || element.Visibility != Visibility.Visible) {
                        return TraverseBehavior.ContinueSkipChildren;
                    }

                    if (!passedSelectionBox) {
                        var boundingBox = element.GetBoundingBox(Owner);
                        if (boundingBox.IsBelow(selectionRectangle)) {
                            return TraverseBehavior.ContinueSkipChildren;
                        }
                        else if (boundingBox.IsAbove(selectionRectangle)) {
                            firstAboveElement = element;
                            passedSelectionBox = true;
                            return TraverseBehavior.Continue;
                        }
                        if (boundingBox.IntersectsWith(selectionRectangle)) {
                            return TraverseBehavior.Continue;
                        }

                            // The bounding box of the element is not above or below the selection rectangle nor intersects with it, meaning it's 
                            // left or right of the rectangle, in which case we continue to check it's descendants.
                            return TraverseBehavior.Continue;
                    }
                    else {
                        if (element.IsVisualChildOf(firstAboveElement)) {
                            return TraverseBehavior.Continue;
                        }
                        return TraverseBehavior.Stop;
                    }
                },
                targetAction: o => {
                    if (o.GetType() == TargetType &&
                      ((FrameworkElement)o).GetBoundingBox(Owner).IntersectsWith(selectionRectangle)) {
                        selectedItems.Add((FrameworkElement)o);
                    }
                }
            );

            UpdateElements(selectedItems);
        }

        public void EndSelection()
        {
            IsActiveDragging = false;
            SelectedItems.Clear();
            SelectedItems = null;
        }



        
        private void ClearSelection()
        {
            VisualTraverser.Traverse
            (
                root: Owner,
                filter: o => TraverseBehavior.Continue,
                targetAction: o => o.SetValue(TargetSelectionProperty, false)
            );
        }

        private void UpdateElements(ISet<FrameworkElement> selectedItems)
        {
            var added = selectedItems.Except(SelectedItems);
            var removed = SelectedItems.Except(selectedItems);

            foreach(var element in added) {
                element.SetValue(TargetSelectionProperty, true);
            }

            foreach(var element in removed) {
                element.SetValue(TargetSelectionProperty, false);
            }

            SelectedItems = selectedItems;
        }
        
    }
}
