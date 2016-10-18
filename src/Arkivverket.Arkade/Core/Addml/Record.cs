using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class Record
    {
        public AddmlRecordDefinition Definition { private set; get; }

        public List<Field> Fields { private set; get; }

        public Record(AddmlRecordDefinition definition, List<Field> fields)
        {
            Definition = definition;
            Fields = fields;
        }
    }
}