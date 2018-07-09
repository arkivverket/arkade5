using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class RecordIndex : IAddmlIndex
    {
        private readonly string _flatFileDefinitionName;
        private readonly string _recordDefinitionName;

        public RecordIndex(string flatFileDefinitionName, string recordDefinitionName)
        {
            Assert.AssertNotNullOrEmpty("flatFileDefinitionName", flatFileDefinitionName);
            Assert.AssertNotNullOrEmpty("recordDefinitionName", recordDefinitionName);

            _flatFileDefinitionName = flatFileDefinitionName;
            _recordDefinitionName = recordDefinitionName;
        }

        public string GetFlatFileDefinitionName()
        {
            return _flatFileDefinitionName;
        }

        public string GetRecordDefinitionName()
        {
            return _recordDefinitionName;
        }

        protected bool Equals(RecordIndex other)
        {
            return string.Equals(_flatFileDefinitionName, other._flatFileDefinitionName) &&
                   string.Equals(_recordDefinitionName, other._recordDefinitionName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecordIndex)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _flatFileDefinitionName.GetHashCode();
                hashCode = (hashCode * 397) ^ _recordDefinitionName.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{_flatFileDefinitionName}/{_recordDefinitionName}";
        }
    }
}
