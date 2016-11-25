using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class Separator
    {
        private static readonly Dictionary<string, string> SpecialSeparators = new Dictionary<string, string>
            {
                {"CRLF", "\r\n"}
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

        private string Convert(string s)
        {
            return SpecialSeparators.ContainsKey(s) ? SpecialSeparators[s] : s;
        }

        internal int GetLength()
        {
            return _separator.Length;
        }
    }
}