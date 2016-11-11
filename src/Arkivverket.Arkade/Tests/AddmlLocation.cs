using System.Text;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Tests
{
    public class AddmlLocation : ILocation
    {
        private readonly string _file;
        private readonly string _record;
        private readonly string _field;

        public AddmlLocation(string file, string record, string field)
        {
            _file = file;
            _record = record;
            _field = field;
        }

        public static AddmlLocation FromFieldIndex(FieldIndex fieldIndex)
        {
            return new AddmlLocation(fieldIndex.GetFlatFileDefinitionName(), fieldIndex.GetRecordDefinitionName(),
                fieldIndex.GetFieldDefinitionName());
        }
        public static AddmlLocation FromRecordIndex(RecordIndex index)
        {
            return new AddmlLocation(index.GetFlatFileDefinitionName(), index.GetRecordDefinitionName(), null);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(_file);

            if (_record != null)
            {
                builder.Append("/").Append(_record);
            }

            if (_field != null)
            {
                builder.Append("/").Append(_field);
            }

            return builder.ToString();
        }
    }
}