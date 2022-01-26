using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class Record
    {
        public AddmlRecordDefinition Definition { get; }

        public List<Field> Fields { get; }

        public long LineNumber { get; }

        public string Value
        {
            get
            {
                string fieldSeparator = Definition.AddmlFlatFileDefinition.FieldSeparator?.Get() ?? "";
                return string.Join(fieldSeparator, Fields.Select(f => f.Value));
            }
        }

        public Record(AddmlRecordDefinition definition, long lineNumber, List<Field> fields)
        {
            Definition = definition;
            Fields = fields;
            LineNumber = lineNumber;
        }

        public Record(AddmlRecordDefinition definition, long lineNumber, params Field[] fields)
        {
            Definition = definition;
            Fields = fields.ToList();
            LineNumber = lineNumber;
        }
    }
}