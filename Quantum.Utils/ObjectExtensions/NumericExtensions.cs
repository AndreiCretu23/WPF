using System;
using System.Diagnostics;

namespace Quantum.Utils
{
    public static class NumericExtensions
    {
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
            if(obj.IsNumeric()) {
                return obj;
            }
            throw new Exception("Error : Was expecting a numeric type.");
        }

        [DebuggerHidden]
        public static bool ToBool<T>(this T numericValue)
            where T : struct, IComparable
        {
            numericValue.AssertNotNull("Value");
            numericValue.AssertNumeric();

            if(numericValue.CompareTo(0) <= 0) {
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
    }
}
