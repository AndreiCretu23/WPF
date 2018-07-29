using Quantum.Exceptions;
using System;
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
        public static void AssertTypeHasGuid(this Type type, string message = null)
        {
            type.AssertNotNull(nameof(type));

            message = message ?? $"Error : {type.Name} does not have a GUID specified via attribute.";
            if(type.GetGuid() == null) {
                throw new Exception(message);
            }
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
        public static bool Implements(this Type type, Type interfaceType)
        {
            type.AssertNotNull(nameof(type));
            interfaceType.AssertParameterNotNull(nameof(interfaceType));
            return interfaceType.IsInterface && interfaceType.IsAssignableFrom(type);
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
        
        public static bool IsSubclassOfRawGeneric(this Type type, Type rawGenericType)
        {
            while(type != null && type != typeof(object)) {
                var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if(rawGenericType == current) {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }

        public static Type GetBaseTypeGenericArgument(this Type type, Type rawBaseType) {
            type.AssertNotNull(nameof(type));
            rawBaseType.AssertParameterNotNull(nameof(rawBaseType));
            if(!rawBaseType.IsGenericType) {
                throw new TypeConflictException(rawBaseType, "Expected a generic type");
            }
            if(rawBaseType.GetGenericArguments().Count() > 1) {
                throw new TypeConflictException(rawBaseType, "Expected a type with a single generic type argument");
            }
            
            while(type != null && type != typeof(object)) {
                var currentRawType = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if(rawBaseType == currentRawType && currentRawType.IsGenericType) {
                    return type.GetGenericArguments().Single();
                }
                type = type.BaseType;
            }

            throw new TypeNotFoundException($"Error : Type {type.Name} does not extend {rawBaseType.Name}");
        }
    }
}
