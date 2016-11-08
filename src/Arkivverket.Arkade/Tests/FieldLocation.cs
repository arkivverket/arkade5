using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Tests
{
    public class FieldLocation : ILocation
    {
        private readonly string _field;
        private readonly string _file;
        private readonly string _record;

        public FieldLocation(string file, string record, string field)
        {
            _file = file;
            _record = record;
            _field = field;
        }

        public static FieldLocation FromFieldIndex(FieldIndex fieldIndex)
        {
            return new FieldLocation(fieldIndex.GetFlatFileDefinitionName(), fieldIndex.GetRecordDefinitionName(),
                fieldIndex.GetFieldDefinitionName());
        }

        public override string ToString()
        {
            return $"{_field}/{_file}/{_record}";
        }
    }
}