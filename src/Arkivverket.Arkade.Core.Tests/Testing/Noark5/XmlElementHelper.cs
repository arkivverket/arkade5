using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Testing;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    internal class XmlElementHelper
    {
        private readonly List<XmlElement> _elements = new List<XmlElement>();

        public override string ToString()
        {
            return new XmlWriter().Write(_elements);
        }

        public XmlElementHelper Add(XmlElement element)
        {
            _elements.Add(element);
            return this;
        }

        public XmlElementHelper Add(string name, XmlElementHelper childHelper)
        {
            Add(name, new string[0], childHelper);
            return this;
        }

        public XmlElementHelper Add(string name, string value)
        {
            Add(name, new string[0], value);
            return this;
        }

        public XmlElementHelper Add(string name, string[] attributes, XmlElementHelper childHelper)
        {
            _elements.Add(new StartElement(name));
            _elements.AddRange(CreateAttributeElements(attributes));
            _elements.AddRange(childHelper._elements);
            _elements.Add(new EndElement(name));
            return this;
        }

        public XmlElementHelper Add(string name, string[] attributes, string value)
        {
            _elements.Add(new StartElement(name));
            _elements.AddRange(CreateAttributeElements(attributes));
            _elements.Add(new ValueElement(value));
            _elements.Add(new EndElement(name));
            return this;
        }
        
        private static IEnumerable<AttributeElement> CreateAttributeElements(string[] attributes)
        {
            var attributeElements = new List<AttributeElement>();

            for (var i = 0; i < attributes.Length - 1; i += 2)
                attributeElements.Add(new AttributeElement(attributes[i], attributes[i + 1]));

            return attributeElements;
        }

        public TestRun RunEventsOnTest(INoark5Test noark5TestClass)
        {
            var path = new Stack<string>();
            foreach (var element in _elements)
            {
                switch (element.ElementType)
                {
                    case ElementType.Start:
                        path.Push(element.Name);
                        noark5TestClass.OnReadStartElementEvent(this, element.AsEventArgs(path));
                        break;
                    case ElementType.Attribute:
                        noark5TestClass.OnReadAttributeEvent(this, element.AsEventArgs(path));
                        break;
                    case ElementType.Value:
                        noark5TestClass.OnReadElementValueEvent(this, element.AsEventArgs(path));
                        break;
                    case ElementType.End:
                        path.Pop();
                        noark5TestClass.OnReadEndElementEvent(this, element.AsEventArgs(path));
                        break;
                }
            }
            return noark5TestClass.GetTestRun();
        }
    }

    internal class StartElement : XmlElement
    {
        public StartElement(string name)
        {
            Name = name;
            ElementType = ElementType.Start;
        }
    }
    internal class AttributeElement : XmlElement
    {
        public AttributeElement(string name, string value)
        {
            Name = name;
            Value = value;
            ElementType = ElementType.Attribute;
        }
    }
    internal class ValueElement : XmlElement
    {
        public ValueElement(string value)
        {
            Value = value;
            ElementType = ElementType.Value;
        }
    }
    internal class EndElement : XmlElement
    {
        public EndElement(string name)
        {
            Name = name;
            ElementType = ElementType.End;
        }
    }
    internal class XmlElement
    {
        public string Name;
        public string Value;
        public ElementType ElementType;

        public ReadElementEventArgs AsEventArgs(Stack<string> path)
        {
            return new ReadElementEventArgs(Name, Value, new ElementPath(path.ToList()));
        }

        public override string ToString()
        {
            if (ElementType == ElementType.Start)
                return $"<{Name}>\n";
            else if (ElementType == ElementType.Attribute)
                return Name + "=\"" + Value + "\"";
            else if (ElementType == ElementType.Value)
                return Value + "\n";
            else
                return $"</{Name}>\n";
        }
    }

    enum ElementType { Start, Attribute, Value, End }

    internal class XmlWriter
    {
        public string Write(List<XmlElement> elements)
        {
            var builder = new StringBuilder();

            for (int elementIndex = 0; elementIndex < elements.Count; elementIndex++)
            {
                var element = elements[elementIndex];

                if (NextIsAttribute(elements, elementIndex))
                {
                    var elementWithAttributes = new StringBuilder($"<{element.Name}");

                    while (NextIsAttribute(elements, elementIndex))
                        elementWithAttributes.Append($" {elements[++elementIndex]}");

                    elementWithAttributes.Append(">\n");

                    builder.Append(elementWithAttributes);
                }
                else
                    builder.Append(element);
            }

            return builder.ToString();
        }

        private static bool NextIsAttribute(IReadOnlyList<XmlElement> elements, int elementIndex)
        {
            return elementIndex < elements.Count - 1 && elements[elementIndex + 1].ElementType == ElementType.Attribute;
        }
    }
}
