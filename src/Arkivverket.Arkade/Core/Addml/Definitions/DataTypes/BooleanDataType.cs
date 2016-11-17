using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    public class BooleanDataType : DataType
    {
        private readonly string _fieldFormat;
        private readonly string _trueString;
        private readonly string _falseString;

        public BooleanDataType(string fieldFormat) : this(fieldFormat, null)
        {            
        }

        public BooleanDataType(string fieldFormat, List<string> nullValues) : base(nullValues)
        {

            string[] strings = fieldFormat.Split('/');
            if (strings.Length != 2)
            {
                ThrowException(fieldFormat);
            }

            _trueString = strings[0];
            _falseString = strings[1];

            if (string.IsNullOrWhiteSpace(_trueString) 
                || string.IsNullOrWhiteSpace(_falseString)
                || _trueString.Equals(_falseString))
            {
                ThrowException(fieldFormat);
            }

            _fieldFormat = fieldFormat;
        }

        private void ThrowException(string fieldFormat)
        {
            throw new ArgumentException("Boolean field format must be in format '<true string>/<false string>'. Was: " + fieldFormat);
        }

        public string GetTrue()
        {
            return _trueString;
        }

        public string GetFalse()
        {
            return _falseString;
        }

        public bool IsValid(string s)
        {
            return s == GetTrue() || s == GetFalse() || IsNull(s);
        }

        protected bool Equals(BooleanDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BooleanDataType) obj);
        }

        public override int GetHashCode()
        {
            return (_fieldFormat != null ? _fieldFormat.GetHashCode() : 0);
        }

    }
}
