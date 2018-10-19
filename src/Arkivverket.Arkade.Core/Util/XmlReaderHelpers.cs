using System.Xml;

namespace Arkivverket.Arkade.Core.Util
{
    public static class XmlReaderHelpers
    {

        public static bool IsNodeTypeAndName(this XmlReader reader, XmlNodeType xmlNodeType, string name)
        {
            return (reader.NodeType == XmlNodeType.Element && reader.Name == name);
        }

        public static bool ForwardToFirstInstanceOfElement(this XmlReader reader, string tagName)
        {
            bool isFoundElementWithName = false;

            while (reader.Read() && isFoundElementWithName == false)
            {
                if (reader.IsNodeTypeAndName(XmlNodeType.Element, tagName))
                {
                    isFoundElementWithName = true;
                }
            }
            return isFoundElementWithName;
        }


        public static bool IsReaderAtElementEndTag(this XmlReader reader, string tagName)
        {
            return (reader.Name == tagName && reader.NodeType == XmlNodeType.EndElement);
        }

    }
}
