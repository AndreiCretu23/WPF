using System;
using System.Collections.Generic;

namespace Quantum.Utils
{
    public class GenericComparer<T> : IEqualityComparer<T>
    {
        public Func<T, T, bool> Compare { get; }
        public Func<T, int> GetHash { get; }

        public GenericComparer(Func<T, T, bool> compare, Func<T, int> getHash)
        {
            getHash.AssertParameterNotNull(nameof(getHash));
            compare.AssertParameterNotNull(nameof(compare));
            Compare = compare;
            GetHash = getHash;
        }

        public bool Equals(T x, T y)
        {
            return Compare(x, y);
        }

        public int GetHashCode(T obj)
        {
            return GetHash(obj);
        }
    }
}
