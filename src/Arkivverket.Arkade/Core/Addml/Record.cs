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
                string recordSeparator = Definition.AddmlFlatFileDefinition.FieldSeparator?.Get() ?? "";
                return string.Join(recordSeparator, Fields.Select(f => f.Value));
            }
        }

        public Record(AddmlRecordDefinition definition, List<Field> fields)
        {
            Definition = definition;
            Fields = fields;
        }
    }
}