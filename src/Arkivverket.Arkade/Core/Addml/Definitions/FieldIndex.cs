using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    internal class FieldIndex
    {
        private readonly string _flatFileDefinitionReference;
        private readonly string _recordDefinitionReference;
        private readonly string _fieldDefinitionReference;

        public FieldIndex(string flatFileDefinitionReference, string recordDefinitionReference,
            string fieldDefinitionReference)
        {
            Assert.AssertNotNullOrEmpty("flatFileDefinitionReference", flatFileDefinitionReference);
            Assert.AssertNotNullOrEmpty("recordDefinitionReference", recordDefinitionReference);
            Assert.AssertNotNullOrEmpty("fieldDefinitionReference", fieldDefinitionReference);

            _flatFileDefinitionReference = flatFileDefinitionReference;
            _recordDefinitionReference = recordDefinitionReference;
            _fieldDefinitionReference = fieldDefinitionReference;
        }

        public FieldIndex(flatFileDefinition flatFileDefinition, recordDefinition recordDefinition, fieldDefinition fieldDefinition)
            : this(flatFileDefinition.name, recordDefinition.name, fieldDefinition.name)
        {            
        }

        public FieldIndex(flatFileDefinitionReference flatFileDefinitionReference, recordDefinitionReference recordDefinitionReference, fieldDefinitionReference fieldDefinitionReference)
            : this(flatFileDefinitionReference.name, recordDefinitionReference.name, fieldDefinitionReference.name)
        {
        }

        protected bool Equals(FieldIndex other)
        {
            return string.Equals(_flatFileDefinitionReference, other._flatFileDefinitionReference) &&
                   string.Equals(_recordDefinitionReference, other._recordDefinitionReference) &&
                   string.Equals(_fieldDefinitionReference, other._fieldDefinitionReference);
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
                int hashCode = _flatFileDefinitionReference.GetHashCode();
                hashCode = (hashCode*397) ^ _recordDefinitionReference.GetHashCode();
                hashCode = (hashCode*397) ^ _fieldDefinitionReference.GetHashCode();
                return hashCode;
            }
        }
    }
}