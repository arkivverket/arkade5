using System.Xml;

namespace Arkivverket.Arkade.Util
{
    public static class XmlReaderHelpers
    {

        public static bool IsNodeTypeAndName(this XmlReader reader, XmlNodeType xmlNodeType, string name)
        {
            return (reader.NodeType == XmlNodeType.Element && reader.Name == name);
        }
    }
}
