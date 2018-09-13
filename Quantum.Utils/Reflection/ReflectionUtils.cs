using Quantum.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Quantum.Utils
{
    public static class ReflectionUtils
    {
        #region Private

        private static IEnumerable<Assembly> Assemblies { get; set; }
        private static IEnumerable<Type> Types { get; set; }
        
        private static IEnumerable<Type> GetSafeTypes(Assembly assembly) {
            assembly.AssertParameterNotNull(nameof(assembly));
            try {
                return assembly.GetTypes();
            }
            catch(ReflectionTypeLoadException e) {
                return e.Types.ExcludeDefaultValues();
            }
        }
        
        private static void LoadReflectionInfo()
        {
            Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Types = Assemblies.SelectMany(assembly => GetSafeTypes(assembly));
        }

        static ReflectionUtils()
        {
            LoadReflectionInfo();
        }

        private static class ReflectionErrorMessages
        {
            public static string LambdaMustBePropertyAccess { get { return "Error : Lambda must be a property accessor"; } }
        }

        #endregion Private
        
        [DebuggerHidden]
        public static IEnumerable<Assembly> GetAssemblies()
        {
            return Assemblies;
        }

        [DebuggerHidden]
        public static IEnumerable<Type> GetTypes()
        {
            return Types;
        }

        [DebuggerHidden]
        public static Type GetTypeByGuid(string guid)
        {
            var match = GetTypes().Where(t => t.HasAttribute<GuidAttribute>() && t.GetGuid() == guid);
            if(match.Count() == 0) {
                throw new TypeNotFoundException($"Could not find the type with the guid {guid}");
            }
            else if(match.Count() > 1) {
                Func<string> nameComposer = () =>
                {
                    string typeNames = string.Empty;
                    match.ForEach(t => typeNames += $"{t.Name}, ");
                    return typeNames;
                };
                throw new TypeConflictException(match, $"The types {nameComposer()} have the same Guids.");
            } 
            else
            {
                return match.Single();
            }
        }

        [DebuggerHidden]
        public static string GetPropertyName<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            propertyExpression = propertyExpression.AssertParameterNotNull(nameof(propertyExpression));

            var memberAccess = propertyExpression.Body.SafeCast<MemberExpression>(ReflectionErrorMessages.LambdaMustBePropertyAccess);
            var propertyInfo = memberAccess.Member.SafeCast<PropertyInfo>(ReflectionErrorMessages.LambdaMustBePropertyAccess);
            return propertyInfo.Name;
        }

        [DebuggerHidden]
        public static string GetPropertyName<T, TResult>(Expression<Func<T, TResult>> propertyExpression)
        {
            propertyExpression = propertyExpression.AssertParameterNotNull(nameof(propertyExpression));

            var memberAccess = propertyExpression.Body.SafeCast<MemberExpression>(ReflectionErrorMessages.LambdaMustBePropertyAccess);
            var propertyInfo = memberAccess.Member.SafeCast<PropertyInfo>(ReflectionErrorMessages.LambdaMustBePropertyAccess);
            return propertyInfo.Name;
        }

        [DebuggerHidden]
        public static PropertyInfo GetPropertyInfo<TResult>(Expression<Func<TResult>> propExpression)
        {
            propExpression = propExpression.AssertParameterNotNull(nameof(propExpression));

            var memberAccess = propExpression.Body.SafeCast<MemberExpression>(ReflectionErrorMessages.LambdaMustBePropertyAccess);
            return memberAccess.Member.SafeCast<PropertyInfo>(ReflectionErrorMessages.LambdaMustBePropertyAccess);
        }

        [DebuggerHidden]
        public static PropertyInfo GetPropertyInfo<T, TResult>(Expression<Func<T, TResult>> propExpression)
        {
            propExpression = propExpression.AssertParameterNotNull(nameof(propExpression));

            var memberAccess = propExpression.Body.SafeCast<MemberExpression>(ReflectionErrorMessages.LambdaMustBePropertyAccess);
            return memberAccess.Member.SafeCast<PropertyInfo>(ReflectionErrorMessages.LambdaMustBePropertyAccess);
        }

        
        public static string GetMethodName(Expression<Func<Delegate>> methodExpression)
        {
            methodExpression.AssertParameterNotNull(nameof(methodExpression));
            return GetMethodInfo(methodExpression).Name;
        }

        public static string GetMethodName<T>(Expression<Func<T, Delegate>> methodExpression)
        {
            methodExpression.AssertParameterNotNull(nameof(methodExpression));
            return GetMethodInfo(methodExpression).Name;
        }

        public static MemberInfo GetMethodInfo(Expression<Func<Delegate>> methodExpression)
        {
            methodExpression.AssertParameterNotNull(nameof(methodExpression));

            var unaryExpression = methodExpression.Body.SafeCast<UnaryExpression>();
            var methodCallExpression = unaryExpression.Operand.SafeCast<MethodCallExpression>();
            var methodInfoExpression = methodCallExpression.Object.SafeCast<ConstantExpression>();
            var methodInfo = methodInfoExpression.Value.SafeCast<MemberInfo>();

            return methodInfo;
        }

        public static MemberInfo GetMethodInfo<T>(Expression<Func<T, Delegate>> methodExpression)
        {
            methodExpression.AssertParameterNotNull(nameof(methodExpression));

            var unaryExpression = methodExpression.Body.SafeCast<UnaryExpression>();
            var methodCallExpression = unaryExpression.Operand.SafeCast<MethodCallExpression>();
            var methodInfoExpression = methodCallExpression.Object.SafeCast<ConstantExpression>();
            var methodInfo = methodInfoExpression.Value.SafeCast<MemberInfo>();

            return methodInfo;
        }

        [DebuggerHidden]
        public static string GetPropertyGetterMethodName(PropertyInfo propertyInfo)
        {
            propertyInfo.AssertParameterNotNull(nameof(propertyInfo));
            return $"get_{propertyInfo.Name}";
        }

        [DebuggerHidden]
        public static string GetPropertySetterMethodName(PropertyInfo propertyInfo)
        {
            propertyInfo.AssertParameterNotNull(nameof(propertyInfo));
            return $"set_{propertyInfo.Name}";
        }
    }
}
