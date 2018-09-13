using System;
using System.Linq.Expressions;

namespace Quantum.Services
{
    public interface IConfigManagerService
    {
        /// <summary>
        /// Registers an instance of a type the specified interface is assignable from in the container.
        /// The actual type is which implements the interface is created at runtime. <para></para>
        /// Specifications : <para></para>
        ///     1) The config interface can only contain properties. No methods are allowed. 
        ///                     If methods are required, declare them as extension methods. <para></para>
        ///                     
        ///     2) You can specify default values for various properties by decorating them with
        ///        the System.ComponentModel.DefaultValueAttribute. <para></para>
        ///        
        ///     3) When the application shuts down, the instance of the config property is 
        ///        serialized in AppData/Roaming/"ApplicationName"/Config/"ConfigInterfaceTypeName.xml". <para></para>
        ///        
        ///     4) After the runtime type is created, the path of the xml associated to the specified interface type
        ///        is checked, and if it exists, the resulting instance will have the properties set to the
        ///        values deserialized from the xml file. <para></para>
        ///        
        ///     5) Adding new properties to the config interface preserves the already-saved existing property values.
        ///        Algorithm : If there is a xml file, deserialize it. For each property, if the xml file contains 
        ///                    serialized data related to that property, set the value of the property to the
        ///                    serialized value. Else if there is no serialized data, but a default value is
        ///                    specified via a DefaultValueAttribute, set that value. Else, leave the default
        ///                    value of the type of the property.<para></para>
        ///     
        ///     6) Setting a config property in the resulting config instance (which will be registered in the
        ///        container mapped the the specified config interface type) will raise the ConfigParamChangedEvent,
        ///        which is registered in the IEventAggregator instance of the container.
        /// 
        /// </summary>
        /// <typeparam name="TConfigInterface"></typeparam>
        void RegisterConfigInterface<TConfigInterface>() where TConfigInterface : class;

        /// <summary>
        /// Sets the value of the property specified by given expression in the associated config instance
        /// registered in the container to the given value without raising the ConfigParamChangedEvent.
        /// </summary>
        /// <typeparam name="TConfigInterface"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="propertyValue"></param>
        void SetConfigPropertyInternal<TConfigInterface, TProperty>(Expression<Func<TConfigInterface, TProperty>> propertyExpression, TProperty propertyValue) where TConfigInterface : class;
    }
}
