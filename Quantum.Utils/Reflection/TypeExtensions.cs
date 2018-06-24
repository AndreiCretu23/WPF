﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Quantum.Utils
{
    public static class TypeExtensions
    {
        [DebuggerHidden]
        public static object GetDefaultValue(this Type type)
        {
            return type.AssertParameterNotNull(nameof(type)).IsValueType ? Activator.CreateInstance(type) : null;
        }

        [DebuggerHidden]
        public static string GetGuid(this Type type)
        {
            type.AssertNotNull(nameof(type));

            string guidValue = null;
            type.IfHasAttribute((GuidAttribute guid) =>
            {
                guidValue = guid.Value;
            });
            return guidValue;
        }

        [DebuggerHidden]
        public static bool IsCollection(this Type type)
        {
            type.AssertNotNull(nameof(type));
            return !(type == typeof(string)) && type.IsSubtypeOf<IEnumerable>();
        }

        [DebuggerHidden]
        public static bool IsCollectionOf<T>(this Type type)
        {
            type.AssertNotNull(nameof(type));
            if (type.IsCollection() && type.IsGenericType && type.GetGenericArguments().Length == 1)
            {
                return type.GetGenericArguments().Single().IsSubtypeOf<T>();
            }
            return false;
        }

        [DebuggerHidden]
        public static bool IsSubtypeOf<T>(this Type type)
        {
            type.AssertNotNull(nameof(type));
            return typeof(T).IsAssignableFrom(type);
        }

        [DebuggerHidden]
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            type.AssertNotNull(nameof(type));
            while(type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}
