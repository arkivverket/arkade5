using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class FlatFileIndex : IAddmlIndex
    {
        private readonly string _flatFileDefinitionName;

        public FlatFileIndex(string flatFileDefinitionName)
        {
            Assert.AssertNotNullOrEmpty("flatFileDefinitionName", flatFileDefinitionName);

            _flatFileDefinitionName = flatFileDefinitionName;
        }

        public string GetFlatFileDefinitionName()
        {
            return _flatFileDefinitionName;
        }

        protected bool Equals(FlatFileIndex other)
        {
            return string.Equals(_flatFileDefinitionName, other._flatFileDefinitionName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FlatFileIndex)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _flatFileDefinitionName.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return _flatFileDefinitionName;
        }
    }
}
