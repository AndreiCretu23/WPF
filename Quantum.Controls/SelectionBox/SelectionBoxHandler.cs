using Quantum.Utils;
using System;
using System.Windows;

namespace Quantum.Controls
{
    public class SelectionBoxHandler
    {
        private FrameworkElement Owner { get; }
        private SelectionBox SelectionBox { get; }
        private SelectionBoxElementManager ElementManager { get; }
        private SelectionBoxOwnerUIManager OwnerUIManager { get; }
        
        public SelectionBoxHandler(FrameworkElement owner, SelectionBox selectionBox)
        {
            AssertSelectionBoxProperties(owner, selectionBox);
            SelectionBox = selectionBox;
            Owner = owner;
            ElementManager = new SelectionBoxElementManager(Owner, SelectionBox);
            OwnerUIManager = new SelectionBoxOwnerUIManager(Owner, SelectionBox, ElementManager);
        }
        
        public void Attach()
        {
            OwnerUIManager.Enable();
        }

        public void Detach()
        {
            OwnerUIManager.Disable();
        }

        private void AssertSelectionBoxProperties(FrameworkElement owner, SelectionBox selectionBox)
        {
            owner.AssertParameterNotNull(nameof(owner));
            selectionBox.AssertParameterNotNull(nameof(selectionBox));
            selectionBox.TargetType.AssertParameterNotNull(nameof(SelectionBox.TargetType));
            selectionBox.TargetSelectionProperty.AssertParameterNotNull(nameof(SelectionBox.TargetSelectionProperty));
            selectionBox.VisualTraverser.AssertParameterNotNull(nameof(SelectionBox.VisualTraverser));

            if (!typeof(FrameworkElement).IsAssignableFrom(selectionBox.TargetType)) {
                throw new Exception($"Error : Only FrameworkElements can be targeted by a SelectionBox. The given target type {SelectionBox.TargetType.Name} " +
                                    $"does not extend FrameworkElement.");
            }
            if (selectionBox.TargetSelectionProperty == null ||
                selectionBox.TargetSelectionProperty.ReadOnly ||
                selectionBox.TargetSelectionProperty.PropertyType != typeof(bool)) {
                throw new Exception($"Error : The target selection property represents the property if, which set, selects the UIElement of the item found inside the SelectionBox. " +
                                    $"This cannot be null or readOnly and it's TargetType must be bool.");
            }
        }

    }
}
