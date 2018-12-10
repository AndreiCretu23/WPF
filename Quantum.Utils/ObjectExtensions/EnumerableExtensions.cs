using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Quantum.Utils
{
    public static class EnumerableExtensions
    {

        [DebuggerHidden]
        public static bool AnyOfType<T>(this IEnumerable collection)
        {
            collection.AssertNotNull(nameof(collection));
            return collection.OfType<T>().Any();
        }

        [DebuggerHidden]
        public static IEnumerable<int> Between(int start, int end)
        {
            return Enumerable.Range(start, end - start);
        }

        [DebuggerHidden]
        public static bool ContainsDuplicates<T>(this IEnumerable<T> collection)
        {
            return collection.Distinct().Count() != collection.Count();
        }

        [DebuggerHidden]
        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> collection)
        {
            collection.AssertParameterNotNull(nameof(collection));
            return collection.GroupBy(o => o).Where(o => o.Count() > 1).Select(o => o.Key);
        }

        [DebuggerHidden]
        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            collection.AssertParameterNotNull(nameof(collection));
            comparer.AssertParameterNotNull(nameof(comparer));
            
            return collection.GroupBy(o => o, comparer).Where(o => o.Count() > 1).Select(o => o.Key).ToList();
        }

        [DebuggerHidden]
        public static IEnumerable<object> EmptyIfNull(this IEnumerable collection)
        {
            return (collection ?? Enumerable.Empty<object>()).Cast<object>();
        }

        [DebuggerHidden]
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        [DebuggerHidden]
        public static IEnumerable<T> ExcludeDefaultValues<T>(this IEnumerable<T> collection)
        {
            foreach (var element in collection.AssertNotNull("Enumeration"))
            {
                if (!EqualityComparer<T>.Default.Equals(element, default(T)))
                {
                    yield return element;
                }
            }
        }

        [DebuggerHidden]
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            action.AssertParameterNotNull(nameof(action));
            foreach (var element in collection.AssertNotNull("Enumeration"))
            {
                action(element);
            }
        }

        [DebuggerHidden]
        public static void ForEachIndexed<T>(this IEnumerable<T> collection, Action<int, T> action)
        {
            int index = 0;
            action.AssertParameterNotNull(nameof(action));
            foreach (var item in collection.AssertParameterNotNull("collection"))
            {
                action(index++, item);
            }
        }

        [DebuggerHidden]
        public static bool Includes<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection)
        {
            collection.AssertNotNull("Enumeration");
            foreach (var element in otherCollection.AssertParameterNotNull(nameof(otherCollection)))
            {
                if (!collection.Any(o => o.Equals(element)))
                {
                    return false;
                }
            }
            return true;
        }

        [DebuggerHidden]
        public static int IndexOf<T>(this IEnumerable<T> collection, T obj)
        {
            int index = 0;
            foreach (var item in collection.AssertNotNull("Enumeration"))
            {
                if (item != null && item.Equals(obj) || (item == null && obj == null)) return index;
                index++;
            }
            return -1;
        }

        [DebuggerHidden]
        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> selector)
        {
            int index = 0;
            selector.AssertParameterNotNull(nameof(selector));
            foreach (var item in collection.AssertNotNull("collection"))
            {
                if (selector(item)) return index;
                index++;
            }
            return -1;
        }
        
        [DebuggerHidden]
        public static bool IsSingleElement<T>(this IEnumerable<T> collection)
        {
            collection.AssertNotNull(nameof(collection));
            if (collection is ICollection list) return list.Count == 1; 
            using (var iter = collection.GetEnumerator()) {
                return iter.MoveNext() && !iter.MoveNext();
            }
        }

        [DebuggerHidden]
        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            item.AssertNotNull();
            yield return item;
        }

        [DebuggerHidden]
        public static IEnumerable<T> ToEnumerable<T>(Func<T> getter)
        {
            getter.AssertParameterNotNull(nameof(getter));
            yield return getter();
        }

        [DebuggerHidden]
        public static IEnumerable<object> ToGenericEnumerable(this IEnumerable collection)
        {
            collection.AssertNotNull("Enumeration");
            foreach(var element in collection) {
                yield return element;
            }
        }

        [DebuggerHidden]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            source.AssertNotNull("Enumeration");
            return new HashSet<T>(source);
        }
        
        [DebuggerHidden]
        public static ObservableCollection<object> ToObservableCollection(this IEnumerable collection)
        {
            var observableCollection = new ObservableCollection<object>();
            foreach (var element in collection.AssertNotNull("Enumeration"))
            {
                observableCollection.Add(element);
            }
            return observableCollection;
        }

        [DebuggerHidden]
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            var observableCollection = new ObservableCollection<T>();
            foreach (var element in collection.AssertNotNull("Enumeration"))
            {
                observableCollection.Add(element);
            }
            return observableCollection;
        }

        [DebuggerHidden]
        public static Queue<object> ToQueue(this IEnumerable collection)
        {
            var queue = new Queue<object>();
            foreach (var element in collection.AssertNotNull("Enumeration"))
            {
                queue.Enqueue(element);
            }
            return queue;
        }

        [DebuggerHidden]
        public static Queue<T> ToQueue<T>(this IEnumerable<T> collection)
        {
            var queue = new Queue<T>();
            foreach (var element in collection.AssertNotNull("Enumeration"))
            {
                queue.Enqueue(element);
            }
            return queue;
        }

        [DebuggerHidden]
        public static Stack<object> ToStack(this IEnumerable collection)
        {
            var stack = new Stack<object>();
            foreach (var element in collection.AssertNotNull("Enumeration"))
            {
                stack.Push(element);
            }
            return stack;
        }

        [DebuggerHidden]
        public static Stack<T> ToStack<T>(this IEnumerable<T> collection)
        {
            var stack = new Stack<T>();
            foreach(var element in collection.AssertNotNull("Enumeration"))
            {
                stack.Push(element);
            }
            return stack;
        }
    }
}
