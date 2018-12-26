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

        public static Rect GetBoundingBox(this FrameworkElement element, Visual relativeTo)
        {
            element.AssertParameterNotNull(nameof(element));
            relativeTo.AssertNotNull(nameof(relativeTo));

            var contentElement = element.GetVisualContent();
            var contentElementRectangle = new Rect(0, 0, contentElement.ActualWidth, contentElement.ActualHeight);
            return contentElement.TransformToAncestor(relativeTo).TransformBounds(contentElementRectangle);
        }
    }
}
