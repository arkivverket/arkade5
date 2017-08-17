using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class Record
    {
        public AddmlRecordDefinition Definition { get; }

        public List<Field> Fields { get; }

        public string Value
        {
            get
            {
                string fieldSeparator = Definition.AddmlFlatFileDefinition.FieldSeparator?.Get() ?? "";
                return string.Join(fieldSeparator, Fields.Select(f => f.Value));
            }
        }

        public Record(AddmlRecordDefinition definition, List<Field> fields)
        {
            Definition = definition;
            Fields = fields;
        }

        public Record(AddmlRecordDefinition definition, params Field[] fields)
        {
            Definition = definition;
            Fields = fields.ToList();
        }
    }
}