using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class Field : HasProcesses
    {
        public AddmlFieldDefinition Definition { get; }
        public string Value { get; }

        public Field(AddmlFieldDefinition definition, string value)
        {
            Definition = definition;
            Value = value;
        }

        public string GetName()
        {
            return Definition.Name;
        }

        public List<string> GetProcesses()
        {
            return Definition.Processes;
        }

        public bool IsPartOfForeignKey(AddmlForeignKey foreignKey)
        {
            return foreignKey.ForeignKeys.Contains(Definition);
        }
    }
}