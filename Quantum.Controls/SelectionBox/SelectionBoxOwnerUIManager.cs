using Quantum.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Quantum.Controls
{
    public class SelectionBoxOwnerUIManager
    {
        private FrameworkElement Owner { get; }
        private SelectionBox SelectionBox { get; }
        private SelectionBoxElementManager ElementManager { get; }

        private Type TargetType { get { return SelectionBox.TargetType; } }
        private DependencyProperty TargetSelectionProperty { get { return SelectionBox.TargetSelectionProperty; } }
        private IVisualTraverser VisualTraverser { get { return SelectionBox.VisualTraverser; } }

        private bool isSelecting = false;
        private bool IsSelecting
        {
            get { return isSelecting; }
            set
            {
                isSelecting = value;
                SelectionBox.RaiseEvent(new SelectionStateChangedArgs(SelectionBox.SelectionStateChangedEvent, Owner, SelectionBox, value));
            }
        }

        public SelectionBoxOwnerUIManager(FrameworkElement owner, SelectionBox selectionBox, SelectionBoxElementManager elementManager)
        {
            Owner = owner;
            SelectionBox = selectionBox;
            ElementManager = elementManager;
        }

        public void Enable()
        {
            Owner.MouseLeftButtonDown += OnOwnerMouseLeftButtonDown;
        }

        public void Disable()
        {
            Owner.MouseLeftButtonDown -= OnOwnerMouseLeftButtonDown;
        }


        #region SelectionBoxAdorner

        private SelectionBoxAdorner SelectionBoxAdorner { get; set; }

        private SelectionBoxAdorner CreateSelectionBoxAdorner(FrameworkElement owner, SelectionBox selectionBox, Point startPoint)
        {
            return new SelectionBoxAdorner(owner)
            {
                StartPoint = startPoint,
                Color = selectionBox.Background,
                Border = new Pen(selectionBox.BorderBrush, selectionBox.BorderThickness)
            };
        }

        #endregion SelectionBoxAdorner


        #region Validation

        private bool ValidateSelectionDragSource(object source)
        {
            if (!(source is DependencyObject dependencyObject)) return false;

            return !dependencyObject.GetVisualAncestors(o => o is ButtonBase || o is ScrollBar).Any();
        }

        #endregion Validation


        #region DraggingManagement

        private void OnOwnerMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FinishDrag();

            if (!ValidateSelectionDragSource(e.OriginalSource) || AdornerLayer.GetAdornerLayer(Owner) == null) return;

            Owner.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(OnOwnerMouseMove), true);
            Owner.LostMouseCapture += OnOwnerLostMouseCapture;
            Owner.PreviewMouseUp += OnOwnerPreviewMouseLeftButtonUp;
            Owner.LostFocus += OnOwnerLostFocus;
            SelectionBoxAdorner = CreateSelectionBoxAdorner(Owner, SelectionBox, e.GetPosition(Owner));
            
        }

        private void OnOwnerMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Released) {
                FinishDrag();
                return;
            }

            Point point = e.GetPosition(Owner);
            if (!IsSelecting && SelectionBoxAdorner != null &&
                  (System.Math.Abs(SelectionBoxAdorner.StartPoint.X - point.X) > SystemParameters.MinimumHorizontalDragDistance ||
                  System.Math.Abs(SelectionBoxAdorner.StartPoint.Y - point.Y) > SystemParameters.MinimumVerticalDragDistance)) {

                AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(Owner);

                if (SelectionBoxAdorner != null) {
                    parentAdorner.Remove(SelectionBoxAdorner);
                }

                parentAdorner.Add(SelectionBoxAdorner);

                ElementManager.BeginSelection();
                IsSelecting = true;

                Owner.CaptureMouse();
            }

            if (SelectionBoxAdorner != null && IsSelecting) {

                SelectionBoxAdorner.EndPoint = point;
                SelectionBoxAdorner.InvalidateVisual();

                ElementManager.UpdateSelection(new Rect(SelectionBoxAdorner.StartPoint, SelectionBoxAdorner.EndPoint));
            }

            e.Handled = true;

        }

        private void OnOwnerPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsSelecting) {
                FinishDrag();
                ElementManager.EndSelection();
            }
        }

        private void OnOwnerLostFocus(object sender, RoutedEventArgs e)
        {
            var focusScope = FocusManager.GetFocusScope(Owner);
            var newFocus = (DependencyObject)FocusManager.GetFocusedElement(focusScope);

            if (!newFocus.IsVisualChildOf(Owner)) {
                OnOwnerPreviewMouseLeftButtonUp(sender, null);
            }
        }

        private void OnOwnerLostMouseCapture(object sender, MouseEventArgs e)
        {
            OnOwnerPreviewMouseLeftButtonUp(sender, null);
        }

        private void FinishDrag()
        {
            Owner.RemoveHandler(UIElement.MouseMoveEvent, new MouseEventHandler(OnOwnerMouseMove));
            Owner.LostMouseCapture -= OnOwnerLostMouseCapture;
            Owner.PreviewMouseUp -= OnOwnerPreviewMouseLeftButtonUp;
            Owner.LostFocus -= OnOwnerLostFocus;

            if (SelectionBoxAdorner != null) {
                AdornerLayer parentAdorner = AdornerLayer.GetAdornerLayer(Owner);
                if (parentAdorner != null) {
                    parentAdorner.Remove(SelectionBoxAdorner);
                }
                SelectionBoxAdorner.IsRemoved = true;
                SelectionBoxAdorner = null;
            }
            IsSelecting = false;
            Owner.ReleaseMouseCapture();
        }

        #endregion DraggingManagement

    }
}
