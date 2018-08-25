using System;
using System.Collections.Generic;

namespace Quantum.Utils
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            TValue result;
            if (dictionary.AssertParameterNotNull("dictionary").TryGetValue(key, out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static TResult GetValueOrDefault<TKey, TResult, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TResult> selector, TResult defaultValue = default(TResult))
        {
            dictionary = dictionary.AssertParameterNotNull("dictionary");
            selector = selector.AssertParameterNotNull("selector");

            TValue result;
            if (dictionary.TryGetValue(key, out result))
            {
                return selector(result);
            }
            return defaultValue;
        }

        public static TValue GetValueOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> creator)
        {
            dictionary = dictionary.AssertParameterNotNull("dictionary");
            creator = creator.AssertParameterNotNull("creator");

            TValue result;
            if (dictionary.TryGetValue(key, out result))
            {
                return result;
            }
            return dictionary[key] = creator(key);
        }
    }
}
