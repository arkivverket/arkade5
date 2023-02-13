
namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class LinkDataType : DataType
    {

        public LinkDataType()
        {
        }

        protected bool Equals(LinkDataType other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LinkDataType) obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override bool IsValid(string s, bool isNullable)
        {
            return base.IsValid(s, isNullable);
        }
    }
}
