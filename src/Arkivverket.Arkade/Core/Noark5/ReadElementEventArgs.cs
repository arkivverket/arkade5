using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Noark5
{
    public class ReadElementEventArgs : EventArgs
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ElementPath Path { get; set; }

        public ReadElementEventArgs(string name, string value, ElementPath path)
        {
            Name = name;
            Value = value;
            Path = path;
        }

        public bool NameEquals(string inputName)
        {
            return StringEquals(Name, inputName);
        }

        private bool StringEquals(string input1, string input2)
        {
            return string.Equals(input1, input2, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public class ElementPath
    {
        private readonly List<string> _path;

        public ElementPath(List<string> path)
        {
            _path = path;
        }

        public bool Matches(params string[] elementNames)
        {
            var matches = true;
            for (var i = 0; i < elementNames.Length; i++)
            {
                matches = matches && string.Equals(elementNames[i], _path[i], StringComparison.CurrentCultureIgnoreCase);
            }
            return matches;
        }
    }
}