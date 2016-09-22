using System;
using System.IO;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Util
{
    public class SerializeUtil
    {
        public static T DeserializeFromFile<T>(string pathToFile)
        {
            string objectData = File.ReadAllText(pathToFile);
            return (T)DeserializeFromString(objectData, typeof(T));
        }

        public static T DeserializeFromString<T>(string objectData)
        {
            return (T)DeserializeFromString(objectData, typeof(T));
        }

        private static object DeserializeFromString(string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            TextReader reader = null;
            try
            {
                reader = new StringReader(objectData);

                result = serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                throw new Exception("Error while deserializing xml file: " + objectData, e);
            }
            finally
            {
                reader?.Close();
            }

            return result;
        }

        /// <summary>
        /// Serialize an object to a MemoryStream.
        /// </summary>
        /// <param name="inputObject">The object to serialize</param>
        /// <param name="ns">Optional namespaces to use when serializing</param>
        /// <returns></returns>
        public static Stream SerializeToStream(object inputObject, XmlSerializerNamespaces ns)
        {
            var serializer = new XmlSerializer(inputObject.GetType());
            var stream = new MemoryStream();
            serializer.Serialize(stream, inputObject, ns);
            stream.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
