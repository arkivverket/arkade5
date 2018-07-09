using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class FieldIndex : IAddmlIndex
    {
        private readonly string _flatFileDefinitionName;
        private readonly string _recordDefinitionName;
        private readonly string _fieldDefinitionName;

        public FieldIndex(string flatFileDefinitionName, string recordDefinitionName,
            string fieldDefinitionName)
        {
            Assert.AssertNotNullOrEmpty("flatFileDefinitionName", flatFileDefinitionName);
            Assert.AssertNotNullOrEmpty("recordDefinitionName", recordDefinitionName);
            Assert.AssertNotNullOrEmpty("fieldDefinitionName", fieldDefinitionName);

            _flatFileDefinitionName = flatFileDefinitionName.ToLower();
            _recordDefinitionName = recordDefinitionName.ToLower();
            _fieldDefinitionName = fieldDefinitionName.ToLower();
        }

        public FieldIndex(flatFileDefinition flatFileDefinition, recordDefinition recordDefinition,
            fieldDefinition fieldDefinition)
            : this(flatFileDefinition.name, recordDefinition.name, fieldDefinition.name)
        {
        }
        public FieldIndex(flatFileDefinition flatFileDefinition, recordDefinition recordDefinition,
            fieldDefinitionReference fieldDefinitionReference)
            : this(flatFileDefinition.name, recordDefinition.name, fieldDefinitionReference.name)
        {
        }

        public FieldIndex(flatFileDefinitionReference flatFileDefinitionReference,
            recordDefinitionReference recordDefinitionReference, fieldDefinitionReference fieldDefinitionReference)
            : this(flatFileDefinitionReference.name, recordDefinitionReference.name, fieldDefinitionReference.name)
        {
        }

        public string GetFlatFileDefinitionName()
        {
            return _flatFileDefinitionName;
        }

        public string GetRecordDefinitionName()
        {
            return _recordDefinitionName;
        }

        public string GetFieldDefinitionName()
        {
            return _fieldDefinitionName;
        }

        protected bool Equals(FieldIndex other)
        {
            return string.Equals(_flatFileDefinitionName, other._flatFileDefinitionName) &&
                   string.Equals(_recordDefinitionName, other._recordDefinitionName) &&
                   string.Equals(_fieldDefinitionName, other._fieldDefinitionName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FieldIndex) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _flatFileDefinitionName.GetHashCode();
                hashCode = (hashCode*397) ^ _recordDefinitionName.GetHashCode();
                hashCode = (hashCode*397) ^ _fieldDefinitionName.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{_flatFileDefinitionName}/{_recordDefinitionName}/{_fieldDefinitionName}";
        }
    }
}