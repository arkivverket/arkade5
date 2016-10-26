using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    internal class FieldIndex
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

            _flatFileDefinitionName = flatFileDefinitionName;
            _recordDefinitionName = recordDefinitionName;
            _fieldDefinitionName = fieldDefinitionName;
        }

        public FieldIndex(flatFileDefinition flatFileDefinition, recordDefinition recordDefinition,
            fieldDefinition fieldDefinition)
            : this(flatFileDefinition.name, recordDefinition.name, fieldDefinition.name)
        {
        }

        public FieldIndex(flatFileDefinitionReference flatFileDefinitionReference,
            recordDefinitionReference recordDefinitionReference, fieldDefinitionReference fieldDefinitionReference)
            : this(flatFileDefinitionReference.name, recordDefinitionReference.name, fieldDefinitionReference.name)
        {
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
            if (obj.GetType() != this.GetType()) return false;
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
            return
                $"{nameof(_flatFileDefinitionName)}: {_flatFileDefinitionName}, {nameof(_recordDefinitionName)}: {_recordDefinitionName}, {nameof(_fieldDefinitionName)}: {_fieldDefinitionName}";
        }
    }
}