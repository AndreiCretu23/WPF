using Microsoft.Practices.Composite.Events;
using Quantum.Utils;
using System;
using System.IO;
using System.Reflection;

namespace Quantum.Services
{
    internal class ConfigTypeHelper
    {
        public Type ConfigInterface { get; }

        public ConfigTypeHelper(Type configInterface)
        {
            ConfigInterface = configInterface.AssertParameterNotNull(nameof(configInterface));
            if(!ConfigInterface.IsInterface)
            {
                throw new Exception(@"Interal Error : ConfigTypeHelper : parameter ""configInterface"" symbolizes the type of the interface this helper is associated to, therefore it must be the type of an interface.");
            }
        }

        #region ConfigTypeInfo

        public string GetConfigImplementationAssembleName()
        {
            return $"{ConfigInterface.Name}Assembly";
        }

        public string GetConfigImplementationTypeName()
        {
            return $"{ConfigInterface.Name}Impl";
        }

        public string GetConfigImplementationPropertyFieldName(PropertyInfo configProperty)
        {
            configProperty.AssertParameterNotNull(nameof(configProperty));
            return $"{configProperty.Name}_Private";
        }

        public string GetConfigImplementationEventAggreagatorFieldName()
        {
            return typeof(EventAggregator).Name;
        }


        #endregion ConfigTypeInfo

        #region ConfigSerializationInfo

        public string GetConfigFilePath()
        {
            return Path.ChangeExtension(Path.Combine(AppInfo.ApplicationConfigRepository, ConfigInterface.Name), ".xml");
        }

        #endregion ConfigSerializationInfo

        #region ConfigInstanceUtils

        public void SetConfigPropertyInternal(object config, PropertyInfo configPropertyInfo, object value)
        {
            config.AssertParameterNotNull(nameof(config));
            configPropertyInfo.AssertParameterNotNull(nameof(configPropertyInfo));
            if(!ConfigInterface.IsAssignableFrom(config.GetType()))
            {
                throw new Exception($"Internal Exception : ");
            }
            if(value != null)
            {
                if(!configPropertyInfo.PropertyType.IsAssignableFrom(value.GetType()))
                {
                    throw new Exception($"Cannot set the value of {ConfigInterface.Name}.{configPropertyInfo.Name} to the specified value. The type of the config property is not assignable from the type of the given value.");
                }
            }
            else if (value == null && configPropertyInfo.PropertyType.IsValueType)
            {
                throw new Exception($"Cannot set the value of {ConfigInterface.Name}.{configPropertyInfo.Name} to null because the type of the property ({configPropertyInfo.PropertyType.Name}) is a value type.");
            }

            var fieldName = GetConfigImplementationPropertyFieldName(configPropertyInfo);
            var field = config.GetType().GetField(fieldName);
            field.SetValue(config, value);
        }

        #endregion ConfigInstanceUtils
    }
}
