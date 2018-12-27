using Quantum.Math;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
            
            VisualTraverser.Traverse
            (
                root: Owner,
                filter: o =>
                {
                    if(!(o is FrameworkElement frameworkElement) || frameworkElement.Visibility != Visibility.Visible) {
                        return VisualTraverseBehavior.Continue;
                    }
                
                    if(frameworkElement.GetBoundingBox(Owner).IntersectsWith(selectionRectangle)) {
                        return VisualTraverseBehavior.Continue | VisualTraverseBehavior.TraverseChildren | VisualTraverseBehavior.Process;
                    }

                    return VisualTraverseBehavior.Continue;
                },
                targetAction: o => {
                    if (o.GetType() == TargetType &&
                      ((FrameworkElement)o).GetContentBoundingBox(Owner).IntersectsWith(selectionRectangle)) {
                        selectedItems.Add((FrameworkElement)o);
                    }
                }
            );

            UpdateElements(selectedItems);
        }

        public void EndSelection()
        {
            IsActiveDragging = false;
            Keyboard.ClearFocus();
            if(SelectedItems.IsSingleElement()) {
                SelectedItems.Single().Focus();
            }
            else {
                Owner.Focus();
            }

            SelectedItems.Clear();
            SelectedItems = null;
        }



        
        private void ClearSelection()
        {
            VisualTraverser.Traverse
            (
                root: Owner,
                filter: o => VisualTraverseBehavior.Continue | VisualTraverseBehavior.TraverseChildren | VisualTraverseBehavior.Process,
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
