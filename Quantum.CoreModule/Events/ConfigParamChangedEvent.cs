using Microsoft.Practices.Composite.Presentation.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Events
{
    public class ConfigParamChangedEvent : CompositePresentationEvent<ConfigParamChangedArgs>
    {
    }

    public class ConfigParamChangedArgs
    {
        public Type ConfigType { get; }
        public string ConfigProperty { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        public ConfigParamChangedArgs(string configProperty, Type configType, object oldValue, object newValue)
        {
            ConfigType = configType;
            ConfigProperty = configProperty;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
