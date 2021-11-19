using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class Field : HasProcesses
    {
        public AddmlFieldDefinition Definition { get; }
        public string Value { get; }

        public Field(AddmlFieldDefinition definition, string value)
        {
            Definition = definition;
            Value = Definition is {AddmlRecordDefinition: {AddmlFlatFileDefinition: { }}}
                ? value.Trim(Definition.AddmlRecordDefinition.AddmlFlatFileDefinition.QuotingChar)
                : value;
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