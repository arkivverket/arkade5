
namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class BooleanDataType : FieldType
    {
        private readonly string _fieldFormat;

        public BooleanDataType(string fieldFormat)
        {
            _fieldFormat = fieldFormat;
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
