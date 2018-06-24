using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Utils
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullEmptyOrWhiteSpace(this string str)
        {
            return str.IsNullOrEmpty() && str.IsNullOrWhiteSpace();
        }
        
        public static TResult IfNotNullOrEmpty<TResult>(this string str, Func<string, TResult> getter, TResult defaultValue = default(TResult))
        {
            getter.AssertParameterNotNull("getter");
            if (!str.IsNullOrEmpty()) {
                return getter(str);
            }
            return defaultValue;
        }

        public static TResult IfNotNullEmptyOrWhiteSpace<TResult>(this string str, Func<string, TResult> getter, TResult defaultValue = default(TResult))
        {
            getter.AssertParameterNotNull("getter");
            if(!str.IsNullEmptyOrWhiteSpace()) {
                return getter(str);
            }
            return defaultValue;
        }
    }
}
