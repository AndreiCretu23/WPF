using System.Windows;

namespace Quantum.Math
{
    public static class RectangleExtensions
    {
        public static bool IsAbove(this Rect rect, Rect otherRect)
        {
            return rect.Bottom < otherRect.Top;
        }

        public static bool IsBelow(this Rect rect, Rect otherRect)
        {
            return rect.Top > otherRect.Bottom;
        }
    }
}
