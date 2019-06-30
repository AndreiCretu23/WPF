using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Quantum.Utils
{
    public static class ListExtensions
    {
        [DebuggerHidden]
        public static void RemoveWhere<T>(this IList<T> list, Predicate<T> condition)
        {
            foreach(var item in list.ToList())
            {
                if(condition(item))
                {
                    list.Remove(item);
                }
            }
        }

    }
}
