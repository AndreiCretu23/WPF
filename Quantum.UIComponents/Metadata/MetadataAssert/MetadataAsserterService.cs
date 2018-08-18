using Quantum.Common;
using Quantum.Exceptions;
using Quantum.Metadata;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Metadata
{
    public class MetadataAsserterService : QuantumServiceBase, IMetadataAsserterService 
    {
        private static readonly IEnumerable<Type> MetadataTypes;
        
        static MetadataAsserterService()
        {
            MetadataTypes = typeof(IMetadataDefinition).Assembly.GetTypes().Where(t => t.IsClass && typeof(IMetadataDefinition).IsAssignableFrom(t));
            foreach(var metadataType in MetadataTypes)
            {
                if(!metadataType.GetCustomAttributes(true).OfType<MandatoryAttribute>().Any()) {
                    throw new MissingAttributeException(metadataType, typeof(MandatoryAttribute), $"Internal Error : {metadataType.Name} does not have a MandatoryAttribute that specifies if the particular metadata is mandatory in a MetadataCollection or not!");
                }
                if(!metadataType.GetCustomAttributes(true).OfType<SupportsMultipleAttribute>().Any()) {
                    throw new MissingAttributeException(metadataType, typeof(SupportsMultipleAttribute), $"Internal Error: { metadataType.Name } does not have a SupportsMultiple that specifies if the particular metadata is supports multiple instances in a MetadataCollection!");
                }
            }

            var metadataCollectionTypes = typeof(MetadataCollection<>).Assembly.GetTypes().Where(t => t.IsAnsiClass && t.IsSubclassOfRawGeneric(typeof(MetadataCollection<>)));
            foreach(var metadataCollectionType in metadataCollectionTypes)
            {
                if(!(metadataCollectionType.GetCustomAttributes(true).OfType<MandatoryCollectionAttribute>().Count() == 1) && 
                    !(metadataCollectionType.IsGenericType && metadataCollectionType.GetGenericTypeDefinition() == typeof(MetadataCollection<>)))
                {
                    throw new MissingAttributeException(metadataCollectionType, typeof(MandatoryCollectionAttribute), $"Internal Error: {metadataCollectionType.Name} does not have a MandatoryCollectionAttribute that specifies if the particular metadata collection type must be set or not within an object containing it as a property.");
                }
            }
        }

        public MetadataAsserterService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        [DebuggerHidden]
        public void AssertMetadataCollectionProperties(object obj, string objName = null)
        {
            objName = objName ?? obj.ToString();

            foreach (var metadataCollectionProperty in obj.GetType().GetProperties().Where(prop => prop.PropertyType.IsSubclassOfRawGeneric(typeof(MetadataCollection<>))))
            {
                var metadataCollectionName = metadataCollectionProperty.Name;
                var metadataCollection = metadataCollectionProperty.GetValue(obj).SafeCast<IEnumerable>().ToGenericEnumerable();
                if (metadataCollection == null)
                {
                    throw new Exception($"Error : {objName}, {metadataCollectionName} must not be set to null. The default internal value is an empty collection." +
                                        $"If the intention is to not have any metadata, simply don't assign any value to the metadataCollection.");
                }

                if (!metadataCollection.Any() && !IsMandatoryCollection(metadataCollectionProperty.PropertyType))
                {
                    continue;
                }

                foreach (var metadataType in MetadataTypes.Where(metadataType => metadataCollectionProperty.PropertyType.GetBaseTypeGenericArgument(typeof(MetadataCollection<>)).IsAssignableFrom(metadataType)))
                {
                    if (IsMandatory(metadataType) && !metadataCollection.Any(metadata => metadata.GetType() == metadataType))
                    {
                        throw new Exception($"Error : {objName}, {metadataCollectionName} does not contain any instance of the mandatory metadata type {metadataType.Name}");
                    }
                    if (!SupportsMultiple(metadataType) && metadataCollection.Count(metadata => metadata.GetType() == metadataType) > 1)
                    {
                        throw new Exception($"Error : {objName}, {metadataCollectionName} contains more than one instance of {metadataType}. This metadata type does not support multiple instances.");
                    }
                }

                foreach(var metadataDef in metadataCollection)
                {
                metadataDef.IfIs((IAssertable assertableDef) => assertableDef.Assert(objName));
                var metadataDefType = metadataDef.GetType();
                if(metadataDefType.IsSubclassOfRawGeneric(typeof(MetadataCollection<>)))
                {
                    GetType().GetMethod("AssertMetadataCollection").MakeGenericMethod(new Type[] { metadataDefType, metadataDefType.GetBaseTypeGenericArgument(typeof(MetadataCollection<>))}).
                        Invoke(this, new object[] { metadataDef, objName });
                }
                }
            }
        }
        
        [DebuggerHidden]
        public void AssertMetadataCollection<TCollection, TDefinition>(TCollection collection, string collectionName = null)
            where TDefinition : IMetadataDefinition
            where TCollection : IEnumerable<TDefinition>
        {
            var objName = collectionName ?? collection.ToString();
            var metadataCollectionName = typeof(TCollection).Name;

            foreach(var metadataType in MetadataTypes.Where(metadataType => metadataType.Implements(typeof(TDefinition))))
            {
                if (IsMandatory(metadataType) && !MetadataCollectionContainsType<TCollection, TDefinition>(collection, metadataType))
                {
                    throw new Exception($"Error : {objName}, {metadataCollectionName} does not contain any instance of the mandatory metadata type {metadataType.Name}");
                }
                if (!SupportsMultiple(metadataType) && CountMetadataTypes<TCollection, TDefinition>(collection, metadataType) > 1)
                {
                    throw new Exception($"Error : {objName}, {metadataCollectionName} contains more than one instance of {metadataType}. This metadata type does not support multiple instances.");
                }
            }

            foreach(var metadataDef in collection)
            {
                metadataDef.IfIs((IAssertable assertableDef) => assertableDef.Assert(collectionName));
                var metadataDefType = metadataDef.GetType();
                if(metadataDefType.IsSubclassOfRawGeneric(typeof(MetadataCollection<>)))
                {
                    GetType().GetMethod("AssertMetadataCollection").MakeGenericMethod(new Type[] { metadataDefType, metadataDefType.GetBaseTypeGenericArgument(typeof(MetadataCollection<>))}).
                        Invoke(this, new object[] { metadataDef, objName });
                }
            }
        }

        #region Utils

        private bool IsMandatory(Type metadataType)
        {
            return metadataType.GetCustomAttributes(false).OfType<MandatoryAttribute>().Single().IsMandatory;
        }

        private bool SupportsMultiple(Type metadataType)
        {
            return metadataType.GetCustomAttributes(false).OfType<SupportsMultipleAttribute>().Single().SupportsMultiple;
        }

        private bool IsMandatoryCollection(Type metadataCollectionType)
        {
            return metadataCollectionType.GetCustomAttributes(false).OfType<MandatoryCollectionAttribute>().Single().IsMandatoryCollection;
        }

        [DebuggerHidden]
        private bool MetadataCollectionContainsType<TCollection, TDefinition>(TCollection collection, Type metadataType)
            where TDefinition : IMetadataDefinition
            where TCollection : IEnumerable<TDefinition>
        {
            if (metadataType.IsGenericType)
            {
                return collection.Where(metadata => metadata.GetType().IsGenericType).Any(metadata => metadata.GetType().GetGenericTypeDefinition() == metadataType.GetGenericTypeDefinition());
            }
            else
            {
                return collection.Any(metadata => metadata.GetType() == metadataType);
            }
        }

        private int CountMetadataTypes<TCollection, TDefinition>(TCollection collection, Type metadataType)
            where TDefinition : IMetadataDefinition
            where TCollection : IEnumerable<TDefinition>
        {
            if (metadataType.IsGenericType)
            {
                return collection.Where(metadata => metadata.GetType().IsGenericType).Count(metadata => metadata.GetType().GetGenericTypeDefinition() == metadataType.GetGenericTypeDefinition());
            }
            else
            {
                return collection.Count(metadata => metadata.GetType() == metadataType);
            }
        }

        #endregion Utils

    }
}
