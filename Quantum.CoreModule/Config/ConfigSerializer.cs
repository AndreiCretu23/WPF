using System;
using System.IO;
using System.Xml.Serialization;

namespace Quantum.Services
{
    internal class ConfigSerializer
    {
        public Type ConfigType { get; }

        public ConfigSerializer(Type configType)
        {
            ConfigType = configType;
        }
        
        public void Serialize(object config, string fileName) 
        {
            var serializer = new XmlSerializer(ConfigType);
            using (var writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, config);
            }
        }

        public object Deserialize(string fileName)
        {
            object deserializedObject = null;

            var serializer = new XmlSerializer(ConfigType);
            using (var reader = new StreamReader(fileName))
            {
                deserializedObject = serializer.Deserialize(reader);
            }

            return deserializedObject;
        }

    }
}
