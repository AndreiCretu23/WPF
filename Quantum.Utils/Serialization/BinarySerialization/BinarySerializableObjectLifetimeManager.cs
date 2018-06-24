using System;
using System.IO;
using System.Windows;

namespace Quantum.Utils
{
    /// <summary>
    /// A class that manages the binary serialization/deserialization of an instance of an object.
    /// When an instance of this class is created, it will attempt to deserialize the object given the fileName.
    /// If the fileName does not exist, it will create a new instance via the defaultValueGetter provided in the constructor.
    /// When the application exists, the object will be serialized.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinarySerializableObjectLifetimeManager<T> where T : class
    {
        public T Value { get; private set; }
        public string FileName { get; private set; }
        private Func<T> DefaultValueGetter { get; set; }

        public BinarySerializableObjectLifetimeManager(string fileName, Func<T> defaultValueGetter)
        {
            this.FileName = fileName;
            this.DefaultValueGetter = defaultValueGetter;

            Deserialize();
            Application.Current.Exit += (sender, e) => Serialize();
        }

        private void Serialize()
        {
            BinarySerializer.Serialize<T>(Value, FileName);
        }

        private void Deserialize()
        {
            try
            {
                Value = BinarySerializer.Deserialize<T>(FileName);
            }
            catch (FileNotFoundException)
            {
                Value = DefaultValueGetter();
            }
        }
    }
}
