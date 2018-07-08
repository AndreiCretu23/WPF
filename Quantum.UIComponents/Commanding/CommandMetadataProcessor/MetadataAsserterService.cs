﻿using Quantum.Exceptions;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Command
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
        }

        public MetadataAsserterService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }
        
        public void AssertMetadata<TCommandContainer>() where TCommandContainer : class, ICommandContainer
        {
            var commandContainer = Container.Resolve<TCommandContainer>();
            var commandContainerName = typeof(TCommandContainer).Name;
            var commandProperties = typeof(TCommandContainer).GetProperties().Where(prop => typeof(IManagedCommand).IsAssignableFrom(prop.PropertyType));

            foreach (var commandProperty in commandProperties)
            {
                var commandName = commandProperty.Name;
                var command = commandProperty.GetValue(commandContainer);
                AssertCommand(command, commandContainerName, commandName);
            }       
        }
        

        public void AssertCommand(object command, string commandContainerName, string commandName)
        {
            foreach (var metadataCollectionProperty in command.GetType().GetProperties().Where(prop => prop.PropertyType.IsSubclassOfRawGeneric(typeof(MetadataCollection<>))))
            {
                var metadataCollectionName = metadataCollectionProperty.Name;
                var metadataCollection = metadataCollectionProperty.GetValue(command).SafeCast<IEnumerable>().ToGenericEnumerable();
                if (metadataCollection == null)
                {
                    throw new Exception($"Error : {commandContainerName}, {commandName}, {metadataCollectionName} must not be set to null. The default internal value is an empty collection." +
                                        $"If the intention is to not have any metadata, simply don't assign any value to the metadataCollection.");
                }

                if (!metadataCollection.Any())
                {
                    continue;
                }

                foreach (var metadataType in MetadataTypes.Where(metadataType => metadataCollectionProperty.PropertyType.GetBaseTypeGenericArgument(typeof(MetadataCollection<>)).IsAssignableFrom(metadataType)))
                {
                    if (IsMandatory(metadataType) && !metadataCollection.Any(metadata => metadata.GetType() == metadataType))
                    {
                        throw new Exception($"Error : {commandContainerName}, {commandName}, {metadataCollectionName} does not contain any instance of the mandatory metadata type {metadataType.Name}");
                    }
                    if (!SupportsMultiple(metadataType) && metadataCollection.Count(metadata => metadata.GetType() == metadataType) > 1)
                    {
                        throw new Exception($"Error : {commandContainerName}, {commandName}, {metadataCollectionName} contains more than one instance of {metadataType}. This metadata type does not support multiple instances.");
                    }
                }
            }
        }


        private bool IsMandatory(Type metadataType)
        {
            return metadataType.GetCustomAttributes(false).OfType<MandatoryAttribute>().Single().IsMandatory;
        }

        private bool SupportsMultiple(Type metadataType)
        {
            return metadataType.GetCustomAttributes(false).OfType<SupportsMultipleAttribute>().Single().SupportsMultiple;
        }
        
    }
}
