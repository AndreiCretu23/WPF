using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Composite.Events;
using Microsoft.Practices.Unity;
using Quantum.Exceptions;
using Quantum.Utils;

namespace Quantum.Services
{
    public class SelectionInitializer : IObjectInitializer
    {
        public IUnityContainer Container { get; set; }

        public void Initialize(object obj)
        {
            var eventAggregator = Container.Resolve<IEventAggregator>();
            var getEventMethod = typeof(IEventAggregator).GetMethod("GetEvent");

            var selectionProperties = obj.GetType().GetProperties().Where(prop => prop.HasAttribute<SelectionAttribute>());
            foreach(var selectionProperty in selectionProperties)
            {
                if(!selectionProperty.PropertyType.IsSubclassOfRawGeneric(typeof(SelectionBase<>)))
                {
                    throw new UnexpectedTypeException(typeof(SelectionBase<>), selectionProperty.PropertyType,
                        $"Error : {obj.GetType().Name}, property {selectionProperty.Name} \n" +
                        $"Only selectionProperties can be decorated with the Selection Attribute. \n" +
                        $"{selectionProperty.Name} is not a selection (it does not inherit from SelectionBase<T>).");
                }

                if(selectionProperty.SetMethod == null || !selectionProperty.SetMethod.IsPublic)
                {
                    throw new MethodAccessException($"Error : {obj.GetType().Name}, property {selectionProperty.Name} \n" +
                                                    $"The selection cannot be resolved because the property's set method is not accesible.");
                }

                var selection = getEventMethod.MakeGenericMethod(selectionProperty.PropertyType).Invoke(eventAggregator, new object[] { });
                selectionProperty.SetValue(obj, selection);
            }
        }

        public void Teardown(object obj)
        {
            var selectionProperties = obj.GetType().GetProperties().Where(prop => prop.HasAttribute<SelectionAttribute>());
            foreach(var selectionProperty in selectionProperties)
            {
                selectionProperty.SetValue(obj, null);
            }
        }
    }
}
