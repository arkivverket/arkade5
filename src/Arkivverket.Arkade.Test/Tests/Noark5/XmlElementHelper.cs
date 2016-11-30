using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    internal class XmlElementHelper
    {
        private readonly List<XmlElement> _elements = new List<XmlElement>();

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var element in _elements)
            {
                builder.Append(element);
            }
            return builder.ToString();
        }
        public XmlElementHelper Add(XmlElement element)
        {
            _elements.Add(element);
            return this;
        }

        public XmlElementHelper Add(string name, XmlElementHelper childHelper)
        {
            _elements.Add(new StartElement(name));
            _elements.AddRange(childHelper._elements);
            _elements.Add(new EndElement(name));
            return this;
        }

        public XmlElementHelper Add(string name, string value)
        {
            _elements.Add(new StartElement(name));
            _elements.Add(new ValueElement(value));
            _elements.Add(new EndElement(name));
            return this;
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
            else if (ElementType == ElementType.Value)
                return Value + "\n";
            else
                return $"</{Name}>\n";
        }
    }

    enum ElementType { Start, Value, End }
}