using Microsoft.Practices.Unity;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Quantum.Services
{
    public static class UnityContainerHelpers
    {
        /// <summary>
        /// Registers the given type in the container as a service. Services have a container controlled lifetime manager, 
        /// so only one instance will be registered. Resolving the type will return the same instance every time.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="container"></param>
        [DebuggerHidden]
        public static void RegisterService<TService>(this IUnityContainer container)
            where TService : class
        {
            container.AssertNotNull(nameof(container));
            container.RegisterType<TService>(new ContainerControlledLifetimeManager());
            container.Resolve<TService>();
        }

        /// <summary>
        /// Registers the given type in the container mapped to the specified interface. Services have a container controlled lifetime manager, 
        /// so only one instance will be registered. Resolving the type will return the same instance every time.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="container"></param>
        [DebuggerHidden]
        public static void RegisterService<TFrom, TTo>(this IUnityContainer container)
            where TTo : class, TFrom
            where TFrom : class
        {
            container.AssertNotNull(nameof(container));
            container.RegisterType<TTo>(new ContainerControlledLifetimeManager());
            container.RegisterType<TFrom, TTo>();
            container.Resolve<TFrom>();
        }

        /// <summary>
        /// Registers the given type in the container as a service. Services have a container controlled lifetime manager, 
        /// so only one instance will be registered. Resolving the type will return the same instance every time.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="container"></param>
        [DebuggerHidden]
        public static void RegisterService(this IUnityContainer container, Type type)
        {
            container.AssertNotNull(nameof(container));
            type.AssertParameterNotNull(nameof(type));

            ContainerTypeAsserter.AssertTypeContainerCompatible(type);

            container.RegisterType(type, new ContainerControlledLifetimeManager());
            container.Resolve(type);
        }


        /// <summary>
        /// Registers the given type in the container mapped to the specified interface. Services have a container controlled lifetime manager, 
        /// so only one instance will be registered. Resolving the type will return the same instance every time.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="container"></param>
        [DebuggerHidden]
        public static void RegisterService(this IUnityContainer container, Type fromType, Type toType)
        {
            container.AssertNotNull(nameof(container));
            fromType.AssertParameterNotNull(nameof(fromType));
            toType.AssertParameterNotNull(nameof(toType));

            ContainerTypeAsserter.AssertTypePairContainerCompatible(fromType, toType);

            container.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
            container.Resolve(fromType);
        }
        
        /// <summary>
        /// Attempts to resolve the requested type from the container. A return value indicates if the resolve process was successful.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryResolve<T>(this IUnityContainer container, out T result)
        {
            container.AssertNotNull(nameof(container));
            try
            {
                result = container.Resolve<T>();
                return true;
            }
            catch(ResolutionFailedException)
            {
                result = default(T);
                return false;
            }
        }
        
    }

    [DebuggerStepThrough]
    internal static class ContainerTypeAsserter
    {
        [DebuggerHidden]
        internal static void AssertTypeContainerCompatible(Type type)
        {
            if (!type.IsClass)
            {
                throw new Exception($"Error : {type.Name} is not a class. You can only register a class as a service.");
            }
            if (type.IsStatic())
            {
                throw new Exception($"Error : {type.Name} is a static class. You cannot register a static class as a service.");
            }
            if (type.IsAbstract)
            {
                throw new Exception($"Error : {type.Name} is an abstract class. You can only register instantiable types in a container.");
            }
        }

        [DebuggerHidden]
        internal static void AssertTypePairContainerCompatible(Type fromType, Type toType)
        {
            AssertTypeContainerCompatible(toType);
            if (!fromType.IsInterface)
            {
                throw new Exception($"Error : {fromType.Name} is not an interface. You can only map a type to an interface.");
            }
            if (!fromType.IsAssignableFrom(toType))
            {
                throw new Exception($"Error : {toType.Name} does not implement {fromType.Name}. In order to register the type {toType} in " +
                    $"the container and map it to {fromType.Name}, {fromType.Name} must be an interface implemented by class {toType.Name}.");
            }
        }
    }
}
