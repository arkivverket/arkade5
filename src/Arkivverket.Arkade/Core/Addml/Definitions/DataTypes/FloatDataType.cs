
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class FloatDataType : DataType
    {
        private readonly string _fieldFormat;

        public FloatDataType()
        {
        }

        public FloatDataType(string fieldFormat, List<string> nullValues) : base(nullValues)
        {
            _fieldFormat = fieldFormat;
        }

        protected bool Equals(FloatDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FloatDataType) obj);
        }

        public override int GetHashCode()
        {
            return (_fieldFormat != null ? _fieldFormat.GetHashCode() : 0);
        }

        public override bool IsValid(string s)
        {
            // TODO: Verify digits, decimal separator and thousands separator
            // n.nnn,nn
            return true;
        }
    }
}
