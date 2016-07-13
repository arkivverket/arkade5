using System;
using System.IO;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Util
{
    public class SerializeUtil
    {
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
            finally
            {
                reader?.Close();
            }

            return result;
        }
    }
}
