﻿using Microsoft.Practices.Composite.Presentation.Events;
using Quantum.Utils;
using System.IO;
using Unity;
using Unity.Lifetime;

namespace Quantum.Services
{
    public static class UnityContainerHelpers
    {
        /// <summary>
        /// Registers a serializable instance of the specified type (Type T must have the Serializable Attribute).
        /// The instance will attempt to be deserialzed and registered into the container. 
        /// If deserialization failed, or if there were no previous use sessions, a new instance will be created.
        /// On application exit, the instance will be serialized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        public static void RegisterConfigType<T>(this IUnityContainer container)
            where T : class, new()
        {
            container.AssertNotNull(nameof(container));
            container.RegisterInstance(new BinarySerializableObjectLifetimeManager<T>(
                                        Path.ChangeExtension(Path.Combine(AppInfo.ApplicationConfigRepository, typeof(T).Name), ".bin"),
                                        () => new T()).Value);
        }

        /// <summary>
        /// Registers a serializable instance of the specified type (Type T must have the Serializable Attribute).
        /// The instance will attempt to be deserialzed and registered into the container. 
        /// If deserialization failed, or if there were no previous use sessions, a new instance will be created.
        /// On application exit, the instance will be serialized.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        public static void RegisterConfigType<TInterface, TType>(this IUnityContainer container)
            where TType : class, TInterface, new()
        {
            container.AssertNotNull(nameof(container));
            container.RegisterInstance<TInterface>(new BinarySerializableObjectLifetimeManager<TType>(
                                                    Path.ChangeExtension(Path.Combine(AppInfo.ApplicationConfigRepository, typeof(TType).Name), ".bin"),
                                                    () => new TType()).Value);
        }


        /// <summary>
        /// Registers the specified event with the given payload into the container.
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <param name="container"></param>
        public static void RegisterEvent<TEventType, TPayload>(this IUnityContainer container)
            where TEventType : CompositePresentationEvent<TPayload>
        {
            container.AssertNotNull(nameof(container));
            container.RegisterType<TEventType>(new ContainerControlledLifetimeManager());
        }



        /// <summary>
        /// Registers the given type in the container as a service. Services have a container controlled lifetime manager, 
        /// so only one instance will be registered. Resolving the type will return the same instance every time.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="container"></param>
        public static void RegisterService<TService>(this IUnityContainer container)
        {
            container.AssertNotNull(nameof(container));
            container.RegisterType<TService>(new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Registers the given type in the container mapped to the specified interface. Services have a container controlled lifetime manager, 
        /// so only one instance will be registered. Resolving the type will return the same instance every time.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="container"></param>
        public static void RegisterService<TFrom, TTo>(this IUnityContainer container)
            where TTo : class, TFrom
        {
            container.AssertNotNull(nameof(container));
            container.RegisterType<TTo>(new ContainerControlledLifetimeManager());
            container.RegisterType<TFrom, TTo>();
        }
    }
}
