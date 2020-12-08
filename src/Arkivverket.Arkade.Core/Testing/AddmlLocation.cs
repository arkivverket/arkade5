using System.Collections.Generic;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Testing
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

        public static AddmlLocation FromFieldIndex(FieldIndex index)
        {
            return new AddmlLocation(index.GetFlatFileDefinitionName(), index.GetRecordDefinitionName(),
                index.GetFieldDefinitionName());
        }
        public static AddmlLocation FromRecordIndex(RecordIndex index)
        {
            return new AddmlLocation(index.GetFlatFileDefinitionName(), index.GetRecordDefinitionName(), null);
        }
        public static AddmlLocation FromFlatFileIndex(FlatFileIndex index)
        {
            return new AddmlLocation(index.GetFlatFileDefinitionName(), null, null);
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

        public static AddmlGroupLocation FromFieldIndex(List<FieldIndex> indexes)
        {
            return new AddmlGroupLocation(indexes);
        }
    }
}