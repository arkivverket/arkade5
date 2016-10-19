using System.Collections.Generic;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class RecordSeparator
    {
        private static readonly Dictionary<string, string> SpecialSeparators = new Dictionary<string, string>
            {
                {"CRLF", "\r\n"}
            };

        public static readonly RecordSeparator CRLF = new RecordSeparator("CRLF");

        private readonly string _name;
        private readonly string _separator;

        public RecordSeparator(string recordSeparator)
        {
            Assert.AssertNotNullOrEmpty("recordSeparator", recordSeparator);

            _name = recordSeparator;
            _separator = Convert(recordSeparator);
        }

        public string Get()
        {
            return _separator;
        }

        public override string ToString()
        {
            return _name;
        }

        protected bool Equals(RecordSeparator other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecordSeparator) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        private string Convert(string s)
        {
            return SpecialSeparators.ContainsKey(s) ? SpecialSeparators[s] : s;
        }
    }
}