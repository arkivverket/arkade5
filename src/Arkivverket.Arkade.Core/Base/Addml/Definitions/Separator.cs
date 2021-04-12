using System.Collections.Generic;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class Separator
    {
        private static readonly Dictionary<string, string> SpecialSeparators = new Dictionary<string, string>
            {
                {"CRLF", "\r\n"},
                {"LF", "\n"}
            };

        public static readonly Separator CRLF = new Separator("CRLF");

        private readonly string _name;
        private readonly string _separator;

        public Separator(string separator)
        {
            Assert.AssertNotNullOrEmpty("separator", separator);

            _name = separator;
            _separator = Convert(separator);
        }

        public string Get()
        {
            return _separator;
        }

        public override string ToString()
        {
            return _name;
        }

        protected bool Equals(Separator other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Separator) obj);
        }

        public override int GetHashCode()
        {
            return _name?.GetHashCode() ?? 0;
        }

        private static string Convert(string separator)
        {
            foreach (string key in SpecialSeparators.Keys)
                if (separator.Contains(key))
                    separator = separator.Replace(key, SpecialSeparators[key]);

            return separator;
        }

        internal int GetLength()
        {
            return _separator.Length;
        }
    }
}