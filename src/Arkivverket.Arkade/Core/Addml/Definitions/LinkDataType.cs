
namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class LinkDataType : FieldType
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
    }
}
