using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class BooleanDataType : DataType
    {
        private readonly string _fieldFormat;
        private readonly string _trueString;
        private readonly string _falseString;

        public BooleanDataType(string fieldFormat, List<string> nullValues = null) : base(fieldFormat, nullValues)
        {

            string[] strings = fieldFormat.Split('/');
            _trueString = strings[0];
            _falseString = strings[1];

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

        public override bool IsValid(string s, bool isNullable)
        {
            return s == GetTrue() || s == GetFalse() || base.IsValid(s, isNullable);
        }

        protected override void VerifyFieldFormat(string fieldFormat)
        {
            string[] strings = fieldFormat.Split('/');
            if (strings.Length != 2)
            {
                ThrowException(fieldFormat);
            }

            string trueString = strings[0];
            string falseString = strings[1];

            if (string.IsNullOrWhiteSpace(trueString)
                || string.IsNullOrWhiteSpace(falseString)
                || trueString.Equals(falseString))
            {
                ThrowException(fieldFormat);
            }
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
            return _fieldFormat?.GetHashCode() ?? 0;
        }

    }
}
