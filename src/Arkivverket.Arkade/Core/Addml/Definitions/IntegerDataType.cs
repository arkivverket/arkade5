
namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class IntegerDataType : FieldType
    {
        public static readonly IntegerDataType Default = new IntegerDataType();

        private string _fieldFormat;

        public IntegerDataType()
        {
        }

        public IntegerDataType(string fieldFormat)
        {
            this._fieldFormat = fieldFormat;
        }

        protected bool Equals(IntegerDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegerDataType) obj);
        }

        public override int GetHashCode()
        {
            return (_fieldFormat != null ? _fieldFormat.GetHashCode() : 0);
        }
    }
}
