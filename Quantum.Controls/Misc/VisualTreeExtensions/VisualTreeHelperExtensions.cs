using Quantum.Utils;
using System.Windows;
using System.Windows.Media;

namespace Quantum.Controls
{
    public static class VisualTreeHelperExtensions
    {
        public static FrameworkElement GetVisualContent(this FrameworkElement element)
        {
            element.AssertParameterNotNull(nameof(element));
            if(element is ICustomContentOwner customContentOwner) {
                return customContentOwner.GetVisualContent();
            }

            return element;
        }

        public static Rect GetContentBoundingBox(this FrameworkElement element, Visual relativeTo)
        {
            element.AssertParameterNotNull(nameof(element));
            relativeTo.AssertNotNull(nameof(relativeTo));

            return element.GetVisualContent().GetBoundingBox(relativeTo);
        }

        public static Rect GetBoundingBox(this FrameworkElement element, Visual relativeTo)
        {
            element.AssertNotNull(nameof(element));
            relativeTo.AssertParameterNotNull(nameof(relativeTo));

            var elementRectangle = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
            return element.TransformToAncestor(relativeTo).TransformBounds(elementRectangle);
        }
    }
}
