using Serilog;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Util
{
    public class SerializeUtil
    {
        private static readonly ILogger _log = Log.ForContext<SerializeUtil>();

        public static T DeserializeFromFile<T>(string pathToFile)
        {
            string objectData = File.ReadAllText(pathToFile);
            return (T)DeserializeFromString(objectData, typeof(T));
        }

        public static T DeserializeFromFile<T>(ArchiveXmlFile archiveXmlFile)
        {
            return DeserializeFromFile<T>(archiveXmlFile.FullName);
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
            catch (InvalidOperationException e)
            {
                string error = e.Message + ": " + e.InnerException?.Message;
                _log.Error(error, e);
                throw new ArkadeException(error);
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

        public static void SerializeToFile(object inputObject, FileInfo targetFileName, XmlSerializerNamespaces ns)
        {
            var serializer = new XmlSerializer(inputObject.GetType());
            using var stream = new StreamWriter(targetFileName.FullName, false, Encoding.UTF8);
            using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true });
            serializer.Serialize(xmlWriter, inputObject, ns);
        }

        public static bool TryDeserializeFromFile<T>(string pathToFile, out T serialized)
        {
            try
            {
                serialized = DeserializeFromFile<T>(pathToFile);
                return true;
            }
            catch
            {
                serialized = default;
                return false;
            }
        }

    }
}
