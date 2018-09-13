using Microsoft.Practices.Composite.Events;
using Quantum.Events;
using Quantum.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;

namespace Quantum.Services
{
    internal class ConfigTypeBuilder
    {
        public Type ConfigInterface { get; private set; }
        private ConfigTypeHelper ConfigHelper { get; }

        public ConfigTypeBuilder(Type configInterface)
        {
            ConfigInterface = configInterface.AssertParameterNotNull(nameof(configInterface));
            AssertConfigInterface();
            ConfigHelper = new ConfigTypeHelper(ConfigInterface);
        }

        #region AssertInterface

        private void AssertConfigInterface()
        {
            
            if (!ConfigInterface.IsInterface)
            {
                throw new Exception($"Error : {ConfigInterface.Name} must be an interface");
            }

            var properties = ConfigInterface.GetProperties();
            var propGetterMethodNames = properties.Select(prop => ReflectionUtils.GetPropertyGetterMethodName(prop));
            var propSetterMethodNames = properties.Select(prop => ReflectionUtils.GetPropertySetterMethodName(prop));
            var propMethodNames = propGetterMethodNames.Concat(propSetterMethodNames);

            if (ConfigInterface.GetMethods().Any(method => !propMethodNames.Contains(method.Name)))
            {
                throw new Exception($"Error : Cannot create config implementation type for {ConfigInterface.Name} because it contains methods. A config type can only contain properties.");
            }
        }

        #endregion AssertInterface


        public Type BuildConfigImplementation()
        {
            var typeBuilder = BuildConfigTypeSkeleton();

            typeBuilder.AddInterfaceImplementation(ConfigInterface);
            BuildConfigBody(typeBuilder);
            
            var concreteType = typeBuilder.CreateType();
            return concreteType;
        }
        

        private TypeBuilder BuildConfigTypeSkeleton()
        {
            // Define the necessary information for the typeBuilder.
            var assemblyName = new AssemblyName(ConfigHelper.GetConfigImplementationAssembleName());
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            // Create the typeBuilder
            var typeBuilder = moduleBuilder.DefineType(ConfigHelper.GetConfigImplementationTypeName(),
                                                    TypeAttributes.Public | TypeAttributes.AutoClass |
                                                    TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                                                    typeof(Object));

            // Add an XmlRootAttribute
            var xmlRootCtor = typeof(XmlRootAttribute).GetConstructor(new Type[] { typeof(string) });
            var xmlRootBuilder = new CustomAttributeBuilder(xmlRootCtor, new object[] { ConfigInterface.Name });
            typeBuilder.SetCustomAttribute(xmlRootBuilder);

            // Create a default, empty constructor.
            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            
            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ret);
            
            return typeBuilder;
        }
        
        private void BuildConfigBody(TypeBuilder typeBuilder)
        {
            var configInterfaceProperties = ConfigInterface.GetProperties();
            var eventAggregatorField = BuildEventAggregatorField(typeBuilder);
            foreach (var prop in configInterfaceProperties)
            {
                var fieldBuilder = BuildConfigPropertyField(typeBuilder, prop);
                var propertyBuilder = BuildConfigProperty(typeBuilder, prop);

                var propGetter = BuildConfigPropertyGetter(typeBuilder, fieldBuilder, prop);
                var propSetter = BuildConfigPropertySetter(typeBuilder, eventAggregatorField, fieldBuilder, prop);

                propertyBuilder.SetGetMethod(propGetter);
                propertyBuilder.SetSetMethod(propSetter);
            }
        }

        private FieldBuilder BuildEventAggregatorField(TypeBuilder typeBuilder)
        {
            var fieldName = ConfigHelper.GetConfigImplementationEventAggreagatorFieldName();
            var fieldBuilder = typeBuilder.DefineField(fieldName, typeof(IEventAggregator), FieldAttributes.Public);

            var xmlIgnoreCtor = typeof(XmlIgnoreAttribute).GetConstructor(Type.EmptyTypes);
            var xmlIgnoreBuilder = new CustomAttributeBuilder(xmlIgnoreCtor, new object[] { });
            fieldBuilder.SetCustomAttribute(xmlIgnoreBuilder);

            return fieldBuilder;
        }
        
        private FieldBuilder BuildConfigPropertyField(TypeBuilder typeBuilder, PropertyInfo configPropertyInfo)
        {
            var fieldName = ConfigHelper.GetConfigImplementationPropertyFieldName(configPropertyInfo);
            var fieldBuilder = typeBuilder.DefineField(fieldName, configPropertyInfo.PropertyType, FieldAttributes.Public);

            var xmlElementCtor = typeof(XmlElementAttribute).GetConstructor(new Type[] { typeof(string), typeof(Type) });
            var xmlElementBuilder = new CustomAttributeBuilder(xmlElementCtor, new object[] { configPropertyInfo.Name, configPropertyInfo.PropertyType });
            fieldBuilder.SetCustomAttribute(xmlElementBuilder);
            
            return fieldBuilder;
        }

        private PropertyBuilder BuildConfigProperty(TypeBuilder typeBuilder, PropertyInfo configPropertyInfo)
        {
            var propertyBuilder = typeBuilder.DefineProperty(configPropertyInfo.Name, PropertyAttributes.None, configPropertyInfo.PropertyType, null);

            var xmlIgnoreCtor = typeof(XmlIgnoreAttribute).GetConstructor(Type.EmptyTypes);
            var xmlIgnoreBuilder = new CustomAttributeBuilder(xmlIgnoreCtor, new object[] { });
            propertyBuilder.SetCustomAttribute(xmlIgnoreBuilder);

            return propertyBuilder;
        }

        private MethodBuilder BuildConfigPropertyGetter(TypeBuilder typeBuilder, FieldBuilder associatedField, PropertyInfo configPropertyInfo)
        {
            var propGetter = typeBuilder.DefineMethod(ReflectionUtils.GetPropertyGetterMethodName(configPropertyInfo),
                                                            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                                            configPropertyInfo.PropertyType, Type.EmptyTypes);

            var il = propGetter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, associatedField);
            il.Emit(OpCodes.Ret);

            return propGetter;
        }

        private MethodBuilder BuildConfigPropertySetter(TypeBuilder typeBuilder, FieldBuilder eventAggregatorField, FieldBuilder associatedField, PropertyInfo configPropertyInfo)
        {
            var propSetter = typeBuilder.DefineMethod(ReflectionUtils.GetPropertySetterMethodName(configPropertyInfo),
                                                            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                                            null, new Type[] { configPropertyInfo.PropertyType });

            var il = propSetter.GetILGenerator();

            var endMethod = il.DefineLabel();
            var eventArgs = il.DeclareLocal(typeof(ConfigParamChangedArgs));
            var oldValue = il.DeclareLocal(configPropertyInfo.PropertyType);
            var newValue = il.DeclareLocal(configPropertyInfo.PropertyType);
            
            // Check if the new value is different from the old value. If not, return.
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, associatedField);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brtrue, endMethod);
            
            // Store the old value in a local variable
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, associatedField);
            il.Emit(OpCodes.Stloc, oldValue);

            // Load the ConfigParamChangedArgs constructor arguments on the evaluation stack.
            il.Emit(OpCodes.Ldstr, configPropertyInfo.Name);
            il.Emit(OpCodes.Ldtoken, ConfigInterface);
            il.Emit(OpCodes.Ldloc, oldValue);
            if (configPropertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, configPropertyInfo.PropertyType);
            }
            il.Emit(OpCodes.Ldarg_1);
            if(configPropertyInfo.PropertyType.IsValueType)
            {
                il.Emit(OpCodes.Box, configPropertyInfo.PropertyType);
            }

            // Get the ConfigParamChangedArgs constructor.
            var cpcCtor = typeof(ConfigParamChangedArgs).GetConstructor(new Type[] { typeof(string), 
                                                                                     typeof(Type),
                                                                                     typeof(object),
                                                                                     typeof(object) });

            // Create the ConfigParamChangedArgs instance and store it in the eventArgs local variable.
            il.Emit(OpCodes.Newobj, cpcCtor);
            il.Emit(OpCodes.Stloc, eventArgs);

            
            // Set the value on the new field.
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, associatedField);

            // Raise the event
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, eventAggregatorField);
            il.Emit(OpCodes.Call, typeof(IEventAggregator).GetMethod(ReflectionUtils.GetMethodName((IEventAggregator e) => (Func<object>)e.GetEvent<ConfigParamChangedEvent>)).MakeGenericMethod(typeof(ConfigParamChangedEvent)));
            il.Emit(OpCodes.Ldloc, eventArgs);
            il.EmitCall(OpCodes.Call, typeof(ConfigParamChangedEvent).GetMethod(ReflectionUtils.GetMethodName((ConfigParamChangedEvent e) => (Action<ConfigParamChangedArgs>)e.Publish)), null);
            
            // Return
            il.Emit(OpCodes.Ret);
            

            // Return Label
            il.MarkLabel(endMethod);
            il.Emit(OpCodes.Ret);
            
            return propSetter;
        }
        
    }
}
