using System.Windows;

namespace Quantum.Utils
{
    public static class UINativeUtils
    {
        public static Point GetCursorPos()
        {
            POINT outValue;
            UINativeUtilsWrapper.GetCursorPos(out outValue);
            return new Point(outValue.X, outValue.Y);
        }
    }
}
