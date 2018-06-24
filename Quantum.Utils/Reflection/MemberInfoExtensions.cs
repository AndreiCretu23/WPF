using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Quantum.Utils
{
    public static class MemberInfoExtensions
    {
        [DebuggerHidden]
        public static bool HasAttribute<TAttribute>(this MemberInfo memberInfo)
            where TAttribute : Attribute
        {
            memberInfo.AssertNotNull(nameof(memberInfo));
            return memberInfo.GetCustomAttributes<TAttribute>(true).Any();
        }

        [DebuggerHidden]
        public static void IfHasAttribute<TAttribute>(this MemberInfo memberInfo, Action<TAttribute> action)
            where TAttribute : Attribute
        {
            memberInfo.AssertNotNull(nameof(memberInfo));
            action.AssertParameterNotNull(nameof(action));
            foreach(var attr in memberInfo.GetCustomAttributes<TAttribute>(true))
            {
                action(attr);
            }
        }

        [DebuggerHidden]
        public static void IfHasSingleAttribute<TAttribute>(this MemberInfo memberInfo, Action<TAttribute> action)
            where TAttribute : Attribute
        {
            memberInfo.AssertNotNull(nameof(memberInfo));
            action.AssertParameterNotNull(nameof(action));
            var attributes = memberInfo.GetCustomAttributes<TAttribute>(true);
            if(attributes.Count() == 1) {
                action(attributes.Single());
            }
            else if(attributes.Count() > 1) {
                throw new InvalidOperationException($"Error : Member {memberInfo.Name} has multiple attributes of the following type.");
            }
        }

        [DebuggerHidden]
        public static TValue GetAttributeValue<TValue, TAttribute>(this MemberInfo memberInfo, Func<TAttribute, TValue> getter, TValue defaultValue = default(TValue), bool inherit = true)
          where TAttribute : Attribute
        {
            memberInfo.AssertParameterNotNull(nameof(memberInfo));
            getter = getter.AssertParameterNotNull(nameof(getter));

            var attr = memberInfo.GetCustomAttribute<TAttribute>(inherit);
            return attr == null ? defaultValue : getter(attr);
        }

        [DebuggerHidden]
        public static bool IsStatic(this MemberInfo memberInfo)
        {
            memberInfo.AssertParameterNotNull(nameof(memberInfo));
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Event: return ((EventInfo)memberInfo).GetAddMethod().IsStatic;
                case MemberTypes.Field: return ((FieldInfo)memberInfo).IsStatic;
                case MemberTypes.Method: return ((MethodInfo)memberInfo).IsStatic;
                case MemberTypes.Property: return ((PropertyInfo)memberInfo).GetGetMethod(false).IsStatic;
                case MemberTypes.Constructor: return ((ConstructorInfo)memberInfo).IsStatic;
                case MemberTypes.TypeInfo: return ((Type)memberInfo).IsAbstract && ((Type)memberInfo).IsSealed;
                case MemberTypes.NestedType: return ((Type)memberInfo).IsAbstract && ((Type)memberInfo).IsSealed;
                default: throw new NotSupportedException();
            }
        }
        
    }
}
