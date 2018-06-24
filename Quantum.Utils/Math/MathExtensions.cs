using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Quantum.Utils;

namespace Quantum.Math
{
    public static class MathExtensions
    {
        #region Private

        [DebuggerHidden]
        private static bool IsNonNullableNumeric<T>(this T obj)
        {
            return (obj is byte) || (obj is sbyte) ||
                   (obj is short) || (obj is ushort) ||
                   (obj is int) || (obj is uint) ||
                   (obj is long) || (obj is ulong) ||
                   (obj is float) ||
                   (obj is double) ||
                   (obj is decimal);
        }

        [DebuggerHidden]
        private static bool IsNullableNumeric<T>(this T obj)
        {
            return (obj is byte?) || (obj is sbyte?) ||
                   (obj is short?) || (obj is ushort?) ||
                   (obj is int?) || (obj is uint?) ||
                   (obj is long?) || (obj is ulong?) ||
                   (obj is float?) ||
                   (obj is double?) ||
                   (obj is decimal?);
        }

        [DebuggerHidden]
        private static bool IsNumeric<T>(this T obj)
        {
            obj.AssertNotNull("obj");
            return obj.IsNonNullableNumeric() || obj.IsNullableNumeric();
        }

        [DebuggerHidden]
        private static T AssertNumeric<T>(this T obj)
        {
            obj.AssertNotNull("obj");
            if (obj.IsNumeric())
            {
                return obj;
            }
            throw new Exception("Error : Was expecting a numeric type.");
        }

        #endregion Private

        [DebuggerHidden]
        public static bool ToBool<T>(this T numericValue)
            where T : struct, IComparable
        {
            numericValue.AssertNotNull("Value");
            numericValue.AssertNumeric();

            if (numericValue.CompareTo(0) <= 0)
            {
                return false;
            }
            return true;
        }

        [DebuggerHidden]
        public static int ToInt(this bool boolean)
        {
            return boolean ? 1 : 0;
        }

        [DebuggerHidden]
        public static bool IsCloseToInfinity(this double value)
        {
            if (Double.IsInfinity(value) || (System.Math.Abs(value) > System.Math.Pow(10d, 10d)))
            {
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public static bool IsInCloseProximityOf(this double value, double otherValue)
        {
            double range = System.Math.Pow(10d, -2);
            double difference = System.Math.Abs(value - otherValue);
            if (difference < range)
            {
                return true;
            }
            return false;
        }

        [DebuggerHidden]
        public static int CutToRange(int value, int min, int max)
        {
            return value < min ?
               min :
               value > max ? max : value;
        }

        [DebuggerHidden]
        public static double CutToRange(double value, double min, double max)
        {
            return value < min ?
               min :
               value > max ? max : value;
        }

        [DebuggerHidden]
        public static float CutToRange(float value, float min, float max)
        {
            return value < min ?
               min :
               value > max ? max : value;
        }

        [DebuggerHidden]
        public static bool IsInRange(int value, int min, int max)
        {
            return min < value && value < max;
        }

        [DebuggerHidden]
        public static bool IsInRange(double value, double min, double max)
        {
            return min < value && value < max;
        }

        [DebuggerHidden]
        public static bool IsInRange(float value, float min, float max)
        {
            return min < value && value < max;
        }

        [DebuggerHidden]
        public static double NaNFallback(double value, double fallback)
        {
            return double.IsNaN(value) ? fallback : value;
        }
    }
}
