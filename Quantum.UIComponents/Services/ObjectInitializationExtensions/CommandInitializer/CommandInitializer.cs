using Microsoft.Practices.Unity;
using Quantum.Command;
using Quantum.Services;
using Quantum.Utils;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Quantum.UIComponents
{
    internal class CommandInitializer : IObjectInitializer
    {
        public IUnityContainer Container { get ; set ; }
        
        public void Initialize(object obj)
        {
            var commandManager = Container.Resolve<ICommandManagerService>();

            foreach (var prop in obj.GetType().GetProperties().Where(p => p.HasAttribute<CommandAttribute>()))
            {
                if(prop.SetMethod == null || !prop.SetMethod.IsPublic)
                {
                    throw new Exception($"Error : {obj.GetType().Name}.{prop.Name} : \n {nameof(CommandAttribute)} : \n " +
                                        $"Cannot assign the associated command because the property does not have a public set method.");
                }

                var cmdAttribute = prop.GetCustomAttributes(true).OfType<CommandAttribute>().Single();
                var commandContainerType = cmdAttribute.CommandContainerType;

                if(commandContainerType == null)
                {
                    throw new Exception($"Error : {obj.GetType().Name}.{prop.Name} : \n {nameof(CommandAttribute)} : \n " +
                                        $"Null is not allowed for the parameter commandContainerType.");
                }

                if(!typeof(ICommandContainer).IsAssignableFrom(commandContainerType))
                {
                    throw new Exception($"Error : {obj.GetType().Name}.{prop.Name} : \n {nameof(CommandAttribute)} : \n " +
                                        $"{commandContainerType.Name} is not a valid command container type. Command containers types are types " +
                                        $"which implement the interface {nameof(ICommandContainer)}");
                }

                var commandContainerMatchingProperties = commandContainerType.GetProperties().Where(containerProp => containerProp.PropertyType == prop.PropertyType &&
                                                                                                                     containerProp.Name == prop.Name);
                if(commandContainerMatchingProperties.Count() != 1)
                {
                    throw new Exception($"Error : {obj.GetType()}.{prop.Name}. \n {nameof(CommandAttribute)} : \n  " +
                                        $"Cannot localize the command {prop.PropertyType.Name} {prop.Name} in commandContainer {commandContainerType.Name}.");
                }

                var commandContainerProperty = commandContainerMatchingProperties.Single();
                
                var expressionArg = Expression.Parameter(commandContainerType);
                var expressionProperty = Expression.Property(expressionArg, commandContainerProperty);


                var expressionBuilder = typeof(Expression).GetMethods().Single(meth => meth.Name == nameof(Expression.Lambda) &&
                                                                                       meth.IsGenericMethod &&
                                                                                       meth.GetGenericArguments().Count() == 1 && 
                                                                                       meth.GetParameters().Count() == 2 && 
                                                                                       meth.GetParameters().First().ParameterType == typeof(Expression) && 
                                                                                       meth.GetParameters().Last().ParameterType == typeof(ParameterExpression[]));

                var funcType = typeof(Func<,>).MakeGenericType(commandContainerType, commandContainerProperty.PropertyType);
                var expression = expressionBuilder.MakeGenericMethod(funcType).Invoke(null, new object[] { expressionProperty, new ParameterExpression[] { expressionArg } });
                
                var commandGetter = typeof(ICommandManagerService).GetMethods().Single(meth => meth.Name == nameof(commandManager.GetCommand) && meth.GetGenericArguments().Count() == 2);
                try
                {
                    var command = commandGetter.MakeGenericMethod(commandContainerType, commandContainerProperty.PropertyType).Invoke(commandManager, new object[] { expression });
                    prop.SetValue(obj, command);
                }
                catch
                {
                    throw new Exception($"Error : Command Container {commandContainerType.Name} is not registered in the CommandManager");
                }
                
            }

        }

        public void Teardown(object obj)
        {
            if (obj == null) return;

            foreach(var prop in obj.GetType().GetProperties().Where(prop => prop.HasAttribute<CommandAttribute>()))
            {
                prop.SetValue(obj, prop.PropertyType.GetDefaultValue());
            }
        }
    }
}
