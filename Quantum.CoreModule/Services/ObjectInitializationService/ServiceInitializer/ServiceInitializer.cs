using System;
using System.Linq;
using Unity;

namespace Quantum.Services
{
    internal class ServiceInitializer : IObjectInitializer
    {
        public IUnityContainer Container { get; set; }

        public void Initialize(object obj)
        {
            var serviceFields = obj.GetType().GetFields().Where(o => o.GetCustomAttributes(true).OfType<ServiceAttribute>().Any());
            var serviceProperties = obj.GetType().GetProperties().Where(o => o.GetCustomAttributes(true).OfType<ServiceAttribute>().Any());
            foreach (var serviceField in serviceFields)
            {
                serviceField.SetValue(obj, Container.Resolve(serviceField.FieldType));
            }
            foreach (var serviceProperty in serviceProperties)
            {
                if (serviceProperty.SetMethod == null || !serviceProperty.SetMethod.IsPublic)
                {
                    throw new Exception($"Error : {obj.GetType().Name}, property {serviceProperty.Name} \n" +
                                        $"The service cannot be resolved because the property's set method is not accesible");
                }
                serviceProperty.SetValue(obj, Container.Resolve(serviceProperty.PropertyType));
            }
        }

        public void Teardown(object obj)
        {

        }
    }
}
