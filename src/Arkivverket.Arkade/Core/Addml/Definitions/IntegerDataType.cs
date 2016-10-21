
namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class IntegerDataType : DataType
    {
        public static readonly IntegerDataType Default = new IntegerDataType();

        public string FieldFormat { get; }

        public IntegerDataType()
        {
        }

        public IntegerDataType(string fieldFormat)
        {
            FieldFormat = fieldFormat;
        }

        protected bool Equals(IntegerDataType other)
        {
            return string.Equals(FieldFormat, other.FieldFormat);
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
            return (FieldFormat != null ? FieldFormat.GetHashCode() : 0);
        }
    }
}
