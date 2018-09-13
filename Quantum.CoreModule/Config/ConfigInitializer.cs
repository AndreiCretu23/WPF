using Quantum.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Quantum.Services
{
    internal class ConfigInitializer
    {
        public Type ConfigInterface { get; }
        public object Config { get; }
        public ConfigTypeHelper ConfigHelper { get; }

        public ConfigInitializer(Type configInterface, object config)
        {
            config.AssertParameterNotNull(nameof(config));
            if(!configInterface.IsInterface || !configInterface.IsAssignableFrom(config.GetType()))
            {
                throw new Exception(@"Internal Error : Parameter ""configType"" must be the type of the config interface implemented by the dynamic type of the config instance.");
            }

            ConfigInterface = configInterface;
            Config = config;
            ConfigHelper = new ConfigTypeHelper(ConfigInterface);
        }

        public void InitializeConfigProperty(PropertyInfo propertyInfo)
        {
            var defaultValueAttribute = propertyInfo.GetCustomAttributes().OfType<DefaultValueAttribute>().SingleOrDefault();
            if (defaultValueAttribute != null)
            {
                var defaultValue = defaultValueAttribute.Value;
                if (defaultValue == null)
                {
                    if (propertyInfo.PropertyType.IsValueType)
                    {
                        throw new Exception($"Error : {ConfigInterface.Name}.{propertyInfo.Name}. DefaultValue specified via attribute is null, but the property is of a value type.");
                    }
                    else
                    {
                        ConfigHelper.SetConfigPropertyInternal(Config, propertyInfo, null);
                    }
                }
                else
                {
                    var defaultValueType = defaultValue.GetType();
                    if (defaultValueType == propertyInfo.PropertyType)
                    {
                        ConfigHelper.SetConfigPropertyInternal(Config, propertyInfo, defaultValue);
                    }
                    else if (propertyInfo.PropertyType.IsAssignableFrom(defaultValueType))
                    {
                        ConfigHelper.SetConfigPropertyInternal(Config, propertyInfo, Convert.ChangeType(defaultValue, propertyInfo.PropertyType));
                    }
                    else
                    {
                        throw new Exception($"Error : {ConfigInterface.Name}.{propertyInfo.Name}. The DefaultValue's type specified via attribute is not convertible to the property type.");
                    }
                }
            }
        }

        public void InitializeConfigInstance()
        {
            var configProperties = ConfigInterface.GetProperties();
            foreach (var prop in configProperties)
            {
                InitializeConfigProperty(prop);
            }
        }        
    }
}
