using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class Field
    {
        public AddmlFieldDefinition Definition { private set; get; }
        public string Value { private set; get; }

        public Field(AddmlFieldDefinition definition, string value)
        {
            Definition = definition;
            Value = value;
        }

        public List<string> GetFieldProcesses()
        {
            return new List<string>();
        }
    }
}