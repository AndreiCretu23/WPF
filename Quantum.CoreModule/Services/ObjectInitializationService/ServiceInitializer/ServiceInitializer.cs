using Microsoft.Practices.Unity;
using Quantum.Utils;
using System;
using System.Linq;

namespace Quantum.Services
{
    internal class ServiceInitializer : IObjectInitializer
    {
        public IUnityContainer Container { get; set; }

        public void Initialize(object obj)
        {
            //var serviceProperties = obj.GetType().GetProperties().Where(o => o.GetCustomAttributes(true).OfType<ServiceAttribute>().Any());
            var serviceProperties = obj.GetType().GetProperties().Where(prop => prop.HasAttribute<ServiceAttribute>());

            foreach (var serviceProperty in serviceProperties)
            {
                if (serviceProperty.SetMethod == null || !serviceProperty.SetMethod.IsPublic)
                {
                    throw new MethodAccessException($"Error : {obj.GetType().Name}, property {serviceProperty.Name} \n" +
                                                    $"The service cannot be resolved because the property's set method is not accesible");
                }
                serviceProperty.SetValue(obj, Container.Resolve(serviceProperty.PropertyType));
            }
        }

        public void Teardown(object obj)
        {
            var serviceProperties = obj.GetType().GetProperties().Where(prop => prop.HasAttribute<ServiceAttribute>());
            foreach(var serviceProperty in serviceProperties)
            {
                serviceProperty.SetValue(obj, null);
            }
        }
    }
}
