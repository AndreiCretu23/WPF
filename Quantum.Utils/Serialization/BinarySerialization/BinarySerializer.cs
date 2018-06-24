using Quantum.Exceptions;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Quantum.Utils
{
    /// <summary>
    /// A wrapper over the BinaryFormatter that makes serialization more simple. 
    /// The Serialized type must have the Serializable Attribute. 
    /// </summary>
    public static class BinarySerializer
    {
        public static void Serialize<T>(T obj, string binaryFileName, bool overWriteIfExists = true)
        {
            if (File.Exists(binaryFileName))
            {
                if (overWriteIfExists)
                {
                    File.Delete(binaryFileName);
                }
                else
                {
                    return;
                }
            }

            using (var stream = new FileStream(binaryFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
            }
        }

        public static T Deserialize<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"Cannot deserialize object of type {typeof(T).Name} : \n " +
                                                $"File {fileName} does not exist!", fileName);
            }

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var formatter = new BinaryFormatter();
                var deserializedInstance = formatter.Deserialize(stream);
                try
                {
                    return deserializedInstance.SafeCast<T>();
                }
                catch (UnexpectedTypeException)
                {
                    throw new UnexpectedTypeException(typeof(T), deserializedInstance.GetType(), $"The instance deserialized from {fileName} is of type {deserializedInstance.GetType().Name} \n" +
                                        $"The expected type was {typeof(T).Name}");
                }
                catch(ArgumentNullException)
                {
                    throw new Exception($"Error : Unable to deserialize the information stored at {fileName}. The resulting instance was null.");
                }
            }
        }
    }
}
