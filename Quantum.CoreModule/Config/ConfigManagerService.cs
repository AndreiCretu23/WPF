using Quantum.Events;
using Quantum.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Quantum.Services
{
    internal class ConfigManagerService : ServiceBase, IConfigManagerService
    {
        
        public ConfigManagerService(IObjectInitializationService initSvc)
            : base(initSvc)
        {
        }

        #region RegisteredTypes

        private List<Type> RegisteredConfigInterfaces = new List<Type>();

        private void AssertConfigTypeNotRegistered<TConfigInterface>()
        {
            if (RegisteredConfigInterfaces.Contains(typeof(TConfigInterface)))
            {
                throw new Exception($"Error : Config type {typeof(TConfigInterface).Name} has already been registered.");
            }
        }

        #endregion RegisteredTypes

        public void RegisterConfigInterface<TConfigInterface>() where TConfigInterface : class
        {
            var configTypeBuilder = new ConfigTypeBuilder(typeof(TConfigInterface));
            var configType = configTypeBuilder.BuildConfigImplementation();

            var configHelper = new ConfigTypeHelper(typeof(TConfigInterface));
            var configFile = configHelper.GetConfigFilePath();

            TConfigInterface configInstance = null;
            if(!File.Exists(configFile))
            {
                configInstance = (TConfigInterface)Activator.CreateInstance(configType);
                var initializer = new ConfigInitializer(typeof(TConfigInterface), configInstance);
                initializer.InitializeConfigInstance();
            }
            else
            {
                var configSerializer = new ConfigSerializer(configType);
                configInstance = (TConfigInterface)configSerializer.Deserialize(configFile);

                var initializer = new ConfigInitializer(typeof(TConfigInterface), configInstance);

                var xmlDoc = XDocument.Load(configFile);
                var root = xmlDoc.Root;
                var xmlConfigProperties = root.Descendants().Where(desc => desc.Parent == root);
                
                foreach(var prop in typeof(TConfigInterface).GetProperties())
                {
                    if(!xmlConfigProperties.Any(node => node.Name == prop.Name))
                    {
                        initializer.InitializeConfigProperty(prop);
                    }
                }
            }

            var eventAggregatorFieldName = configHelper.GetConfigImplementationEventAggreagatorFieldName();
            configType.GetField(eventAggregatorFieldName).SetValue(configInstance, EventAggregator);

            Container.RegisterInstance<TConfigInterface>(configInstance);
            RegisteredConfigInterfaces.Add(typeof(TConfigInterface));
        }

        public void SetConfigPropertyInternal<TConfigInterface, TProperty>(Expression<Func<TConfigInterface, TProperty>> propertyExpression, TProperty propertyValue) where TConfigInterface : class
        {
            propertyExpression.AssertParameterNotNull(nameof(propertyExpression));
            if(!RegisteredConfigInterfaces.Contains(typeof(TConfigInterface)))
            {
                throw new Exception($"Error : The config type {typeof(TConfigInterface).Name} has not been registered.");
            }
            
            var configProperty = ReflectionUtils.GetPropertyInfo(propertyExpression);
            var configHelper = new ConfigTypeHelper(typeof(TConfigInterface));

            var config = Container.Resolve<TConfigInterface>();
            configHelper.SetConfigPropertyInternal(config, configProperty, propertyValue);
        }

        #region OnShutdown

        [Handles(typeof(ShutdownEvent))]
        public void OnShutDown()
        {
            foreach (var configInterface in RegisteredConfigInterfaces)
            {
                var configHelper = new ConfigTypeHelper(configInterface);
                
                var configInstance = Container.Resolve(configInterface);
                var serializer = new ConfigSerializer(configInstance.GetType());
                
                serializer.Serialize(configInstance, configHelper.GetConfigFilePath());
            }
        }

        #endregion OnShutdown

    }
}
